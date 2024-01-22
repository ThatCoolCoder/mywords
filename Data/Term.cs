using System.Collections.Generic;

namespace Data;

public class Term
{
    public long Id { get; set; }
    public long CollectionId { get; set; }

    public string Value { get; set; } = "";
    public string Definition { get; set; } = "";
    public string Notes { get; set; } = "";
    public int CurrentStreak { get; set; }
    public TermList TermList { get; set; } = TermList.Backlog;
    public DateTime MovedToCurrentListUtc { get; set; }

    public Collection Collection { get; set; } = null!;
    public List<LabelTerm> LabelTerms { get; set; } = new();
}