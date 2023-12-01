namespace Data;

public class LabelTerm
{
    public long Id { get; set; }
    public long TermId { get; set; }
    public long LabelId { get; set; }

    public Term Term { get; set; } = null!;
    public Label Label { get; set; } = null!;
}