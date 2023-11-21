namespace Data;

public class Label
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public long TermSetId { get; set; }

    public TermSet? TermSet { get; set; }
}