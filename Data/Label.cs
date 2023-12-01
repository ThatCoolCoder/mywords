using System.Collections.Generic;

namespace Data;

public class Label
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public string Color { get; set; } = "";
    public long TermSetId { get; set; }

    public TermSet TermSet { get; set; } = null!;
    public List<LabelTerm> LabelTerms { get; set; } = new();
}