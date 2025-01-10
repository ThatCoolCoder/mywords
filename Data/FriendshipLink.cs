using System;

namespace Data;

public class FriendshipLink
{
    // Record of a link that someone sends to someone else to start a friendship (instead of searching them up)

    public Guid Id { get; set; } = Guid.NewGuid();
    public string SenderId { get; set; } = "";
    public bool SingleUse { get; set; } = true;

    public ApplicationUser Sender { get; set; } = null!;
}