using Microsoft.AspNetCore.Identity;
using Data;
using Microsoft.Extensions.Options;

namespace Services;

public class ApplicationUserManager : UserManager<ApplicationUser>
{
    private ILogger _logger;

    public ApplicationUserManager(IUserStore<ApplicationUser> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<ApplicationUser> passwordHasher,
        IEnumerable<IUserValidator<ApplicationUser>> userValidators,
        IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors, IServiceProvider services,
        ILogger<UserManager<ApplicationUser>> logger)
        : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
        _logger = logger;
    }

    public ApplicationUser? GetLoggedInUser(HttpContext context)
    {
        return Users.FirstOrDefault(x => x.Email == context.User.Identity!.Name);
    }
}