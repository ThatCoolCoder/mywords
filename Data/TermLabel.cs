namespace Data;

public class TermLabel
{
    public long Id { get; set; }
    public long TermId { get; set; }
    public long LabelId { get; set; }

    public Term? Term { get; set; }
    public Label? Label { get; set; }
}