namespace Data;

public class Term
{
    public long Id { get; set; }
    public long TermSetId { get; set; }

    public string Value { get; set; } = "";
    public string Definition { get; set; } = "";
    public string Notes { get; set; } = "";
    public int CurrentStreak { get; set; }
    public DateTime MovedToCurrentListUtc { get; set; }

    public TermSet? TermSet { get; set; }
}