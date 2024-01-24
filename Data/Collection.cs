namespace Data;

public class Collection
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string ApplicationUserId { get; set; } = "";

    public ApplicationUser? ApplicationUser { get; set; }
    public ICollection<Term> Terms { get; set; } = new List<Term>();
    public ICollection<Label> Labels { get; set; } = new List<Label>();
}