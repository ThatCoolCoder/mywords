namespace Data;

// Possible optimization: normalize by moving statistics/metadata into separate table
public class Term
{
    public long Id { get; set; }
    public long CollectionId { get; set; }

    public string Value { get; set; } = "";
    public string Definition { get; set; } = "";
    public string Notes { get; set; } = "";
    public TermList TermList { get; set; } = TermList.Backlog;

    public int CurrentStreakWithinList { get; set; } // like currentstreak but reset when it changes lists
    public int CurrentStreak { get; set; }
    public int CurrentAntiStreak { get; set; } // Antistreak - how many times you get it wrong
    public int TotalAnswers { get; set; }
    public int TotalCorrectAnswers { get; set; }
    public DateTime MovedToCurrentListUtc { get; set; }
    public DateTime CreatedUtc { get; set; }

    public Collection Collection { get; set; } = null!;
    public List<LabelTerm> LabelTerms { get; set; } = new();
}