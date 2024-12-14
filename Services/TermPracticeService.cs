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
        public float RecentReviewProportion { get; set; } = 0.4f;
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

        var possibleTerms = FilterByLabels(collection, settings);


        int recentReviewAmount = settings.IncludeRecentReview ? (int)(settings.RoundLength * settings.RecentReviewProportion) : 0;
        int lateReviewAmount = settings.IncludeLateReview ? (int)(settings.RoundLength * settings.LateReviewProportion) : 0;
        int normalTermsAmount = settings.RoundLength - recentReviewAmount - lateReviewAmount;

        Console.WriteLine($"{possibleTerms.Count()} rr: {recentReviewAmount}, lr: {lateReviewAmount}, normal: {normalTermsAmount}");

        var resultingTerms = new List<Term>();
        if (settings.IncludeRecentReview)
        {
            resultingTerms.AddRange(GetNRandomTerms(possibleTerms, TermList.RecentlyLearned, recentReviewAmount));
        }
        if (settings.IncludeLateReview)
        {
            // terms which have a smaller ratio of total correct answers to total answers will be ordered earlier so more likely to be picked wait that doesn't work
            resultingTerms.AddRange(GetNRandomTerms(possibleTerms, TermList.Learned, lateReviewAmount));
        }
        resultingTerms.AddRange(GetNRandomTerms(possibleTerms, TermList.Learned, normalTermsAmount));

        // Try adding more terms if failing to find, in a preferred order
        void AddExtraTermsIfRequired(TermList fromList)
        {
            Console.WriteLine($"Attempting to add 2 from {fromList}");
            if (resultingTerms.Count < settings.RoundLength)
                resultingTerms.AddRange(GetNRandomTerms(possibleTerms, fromList, settings.RoundLength - resultingTerms.Count));
        }
        // todo: need to filter out stuff
        // AddExtraTermsIfRequired(TermList.Learning);
        // AddExtraTermsIfRequired(TermList.RecentlyLearned);
        // AddExtraTermsIfRequired(TermList.Learned);


        var random = new Random();
        return resultingTerms.OrderBy(x => random.Next());
    }

    private IEnumerable<Term> FilterByLabels(Collection collection, PracticeSettings settings)
    {
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

        return possibleTerms;
    }

    private IEnumerable<Term> GetNRandomTerms(IEnumerable<Term> terms, TermList termList, int count)
    {
        // It's hard to do this efficiently. Hopefully ef manages to do a majority of this on the db.
        // It is a bit inefficient in that I believe it hits the db separately for each chosen item, but I don't believe it needs to download all the items
        // (as opposed to getting all the items then processing client-side)
        // It's essentially a tradeoff between lots of small requests or one big one
        // I think that's ok as people may have a lot of terms but generally each round won't be that big
        // But if generating rounds becomes slow, look here first

        var items = new List<Term>();
        var random = new Random();

        var availableSortedPool = terms
            .Where(x => x.TermList == termList)
            .OrderBy(t => t.Id);

        var availableCount = terms.Count();
        count = Math.Min(count, availableCount);

        var indexes = new HashSet<int>();
        while (indexes.Count < count)
        {
            var index = random.Next(availableCount);
            if (indexes.Add(index))
            {
                yield return availableSortedPool.Skip(index).First()!;
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
                // Learning terms can be offered to be moved to recently learned when they've been gotten right enough
                if (term.CurrentStreak >= 3) result = PracticeAnswerResult.CanMoveToRecentlyLearned;
                else result = PracticeAnswerResult.StillInSameList;
            }
            else if (term.TermList == TermList.RecentlyLearned)
            {
                // Recently learned terms are moved to learned after they have a streak and have been in this list for some period
                // (if they spam it a bunch then a shorter period is allowed else no)
                var daysRequiredToMoveToLearned = Math.Clamp(7f / term.TotalCorrectAnswers * (2f / (7f / 20f)), 2f, 7f);
                if (term.CurrentStreak >= 3 && (DateTime.UtcNow - term.MovedToCurrentListUtc) > TimeSpan.FromDays(daysRequiredToMoveToLearned))
                {
                    result = PracticeAnswerResult.MovedToLearned;
                    term.TermList = TermList.Learned;
                    term.CurrentStreakWithinList = 0; // todo: not having a single way to reset this & move terms between lists is going to bite me one day
                }
                else
                {
                    result = PracticeAnswerResult.StillInSameList;
                }
            }
            else if (term.TermList == TermList.Learned)
            {
                // Learned terms that are gotten correct require no changing
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
            // Incorrect stuff has the option to be moved to learning if gotten wrong
            term.CurrentStreakWithinList = 0;
            term.CurrentStreak = 0;
            term.CurrentAntiStreak++;
            result = term.TermList == TermList.Learning
                ? PracticeAnswerResult.StillInSameList
                : PracticeAnswerResult.CanReturnToLearning;
        }

        _context.Update(term);
        await _context.SaveChangesAsync();

        return (PracticeAnswerResult)result;
    }
}