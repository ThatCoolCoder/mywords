using System;

namespace Data;

// public enum Friendship

public class Friendship
{
    // User 1 is always the one that initiates, user 2 has to accept/reject

    public string ApplicationUser1Id = "";
    public string ApplicationUser2Id = "";
    public bool Accepted = false;

    public ApplicationUser? ApplicationUser1;
    public ApplicationUser? ApplicationUser2;
}