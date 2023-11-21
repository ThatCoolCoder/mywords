namespace Data;

public class TermSet
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string ApplicationUserId { get; set; } = "";

    public ApplicationUser? ApplicationUser { get; set; }
}