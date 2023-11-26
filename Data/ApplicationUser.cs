using Microsoft.AspNetCore.Identity;

namespace Data;

public class ApplicationUser : IdentityUser
{
    // No custom props here yet but we're bound to need it so might as well make the class now
    public string GivenName { get; set; } = "";
    public string FamilyName { get; set; } = "";
}