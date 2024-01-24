using System.Diagnostics.Tracing;
using Data;

namespace Services;

public class TermPracticeService
{
    // Business logic of practice mode
    // In many ways this is the core of the service provided by the app
    // todo: to fulfill the original plans, we need to make the magic numbers (and potentially some behaviours) user-configurable

    private readonly ApplicationDbContext _context;

    public TermPracticeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public class PracticeSettings
    {
        public List<long>? IncludeLabelIds { get; set; } = null; // if null assumes include all
        public List<long>? ExcludeLabelIds { get; set; } = null; // if null assumes don't exclude any

        public bool IncludeRecentReview { get; set; } = true; // review of recently learned
        public float RecentReviewProportion { get; set; } = 0.25f;
        public bool IncludeLateReview { get; set; } = true; // review of learned
        public float LateReviewProportion { get; set; } = 0.1f;

        public int RoundLength { get; set; } = 10;
    }

    public enum PracticeAnswerResult
    {
        StillInSameList = 0,
        CanMoveToRecentlyLearned = 1,
        CanReturnToLearning = 2,
        MovedToLearned = 3,
    }

    public class TermPracticeException : Exception { }
    public class PracticingBackLogTerm : TermPracticeException { }

    public IEnumerable<Term> GeneratePracticeRound(Collection collection, PracticeSettings settings)
    {
        // Requires collection labels to be loaded

        var availableLabelIds = collection.Labels.Select(l => l.Id).ToHashSet();
        var includeLabelIds = settings.IncludeLabelIds?.Intersect(availableLabelIds).ToHashSet(); // can optimise with using hashset intersect
        var excludeLabelIds = settings.ExcludeLabelIds?.Intersect(availableLabelIds).ToHashSet();

        var possibleTerms = _context.Term
            .Where(t => t.CollectionId == collection.Id);

        if (includeLabelIds != null)
            possibleTerms = possibleTerms
                .Where(t => t.LabelTerms.Select(lt => lt.Id).Intersect(includeLabelIds).Any());
        if (excludeLabelIds != null)
            possibleTerms = possibleTerms
                .Where(t => !t.LabelTerms.Select(lt => lt.Id).Intersect(excludeLabelIds).Any());


        int recentReviewAmount = settings.IncludeRecentReview ? (int)(settings.RoundLength * settings.RecentReviewProportion) : 0;
        int lateReviewAmount = settings.IncludeLateReview ? (int)(settings.RoundLength * settings.LateReviewProportion) : 0;
        int normalTermsAmount = settings.RoundLength - recentReviewAmount - lateReviewAmount;

        var resultingTerms = new List<Term>();
        if (settings.IncludeRecentReview)
        {
            resultingTerms.AddRange(GetNRandomItems(possibleTerms.Where(t => t.TermList == TermList.RecentlyLearned), recentReviewAmount, t => t.Id));
        }
        if (settings.IncludeLateReview)
        {
            // todo: it should weight ones that you have historically gotten correct less often higher
            resultingTerms.AddRange(GetNRandomItems(possibleTerms.Where(t => t.TermList == TermList.Learned), lateReviewAmount, t => t.Id));
        }
        resultingTerms.AddRange(GetNRandomItems(possibleTerms.Where(t => t.TermList == TermList.Learning), normalTermsAmount, t => t.Id));

        return resultingTerms;
    }

    private IEnumerable<T> GetNRandomItems<T>(IEnumerable<T> source, int count, Func<T, long> getId)
    {
        // It's hard to do this efficiently. Hopefully ef manages to do a majority of this on the db.
        // It is a bit inefficient in that I believe it hits the db separately for each chosen item, but I don't believe it needs to download all the items
        // (as opposed to getting all the items then processing client-side)
        // It's essentially a tradeoff between lots of small requests or one big one
        // I think that's ok as people may have a lot of terms but generally each round won't be that big
        // But if generating rounds becomes slow, look here first

        var items = new List<T>();
        var random = new Random();
        var sorted = source.OrderBy(getId);
        var totalCount = source.Count();
        count = Math.Min(count, totalCount);

        var indexes = new HashSet<int>();
        while (indexes.Count < count)
        {
            var index = random.Next(count);
            if (indexes.Add(index))
            {
                yield return sorted.Skip(index).FirstOrDefault()!;
            }
        }
    }

    public async Task<PracticeAnswerResult> SubmitAnswer(Term term, bool correct)
    {
        PracticeAnswerResult? result;

        if (term.TermList == TermList.Backlog) throw new PracticingBackLogTerm();

        if (correct)
        {
            term.CurrentStreakWithinList++;
            term.CurrentStreak++;
            term.CurrentAntiStreak = 0;
            term.TotalCorrectAnswers++;

            if (term.TermList == TermList.Learning)
            {
                if (term.CurrentStreak >= 3) result = PracticeAnswerResult.CanMoveToRecentlyLearned;
                else result = PracticeAnswerResult.StillInSameList;
            }
            else if (term.TermList == TermList.RecentlyLearned)
            {
                if (term.CurrentStreak >= 3 && (DateTime.UtcNow - term.MovedToCurrentListUtc) > TimeSpan.FromDays(2))
                {
                    result = PracticeAnswerResult.MovedToLearned;
                    term.TermList = TermList.Learned;
                    term.CurrentStreakWithinList = 0; // todo: not having a single way to reset this is going to bite me one day
                }
                else
                {
                    result = PracticeAnswerResult.StillInSameList;
                }
            }
            else if (term.TermList == TermList.Learned)
            {
                result = PracticeAnswerResult.StillInSameList;
            }
            else
            {
                // Should never arise, just a catch to stop an else gobbling up cases I forget about
                throw new NotImplementedException();
            }
        }
        else
        {
            term.CurrentStreakWithinList = 0;
            term.CurrentStreak = 0;
            term.CurrentAntiStreak++;
            result = PracticeAnswerResult.CanReturnToLearning;
        }

        _context.Update(term);
        await _context.SaveChangesAsync();

        return (PracticeAnswerResult)result;
    }
}