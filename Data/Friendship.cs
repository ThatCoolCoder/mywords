using System;

namespace Data;

public class Friendship
{
    // User 1 is always the one that initiates, user 2 has to accept/reject

    public long Id { get; set; }
    public string ApplicationUser1Id { get; set; } = "";
    public string ApplicationUser2Id { get; set; } = "";
    public bool Accepted { get; set; } = false;

    public ApplicationUser ApplicationUser1 { get; set; } = null!;
    public ApplicationUser ApplicationUser2 { get; set; } = null!;
}