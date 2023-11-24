using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components.Authorization;

using Data;

void ConfigureServices(IServiceCollection services, ConfigurationManager configuration, bool IsDevelopment)
{
    services.AddCors();
    services.AddControllersWithViews();
    services.AddEndpointsApiExplorer();
    services.AddDbContextFactory<ApplicationDbContext>(options =>
    {
        options.UseNpgsql(configuration["DBConnectionString"]);
        options.UseLowerCaseNamingConvention();
    });
    services.AddScoped<ApplicationDbContext>(p => p.GetRequiredService<IDbContextFactory<ApplicationDbContext>>()
        .CreateDbContext());
    services.AddScoped<SignInManager<ApplicationUser>>();

    services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddUserManager<Services.ApplicationUserManager>()
        .AddRoles<IdentityRole>()
        // .AddDefaultUI()
        .AddEntityFrameworkStores<ApplicationDbContext>();

    // todo: make this less
    services.Configure<IdentityOptions>(options =>
    {
        // Password settings.
        options.Password.RequireDigit = IsDevelopment ? false : true;
        options.Password.RequireLowercase = IsDevelopment ? false : true;
        options.Password.RequireNonAlphanumeric = IsDevelopment ? false : true;
        options.Password.RequireUppercase = IsDevelopment ? false : true;
        options.Password.RequiredLength = IsDevelopment ? 1 : 6;
        options.Password.RequiredUniqueChars = 1;

        // Lockout settings.
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;

        // User settings.
        options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        options.User.RequireUniqueEmail = true;
    });

    // services.ConfigureApplicationCookie(options =>
    // {
    //     // Cookie settings
    //     options.Cookie.HttpOnly = true;
    //     options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    //     options.LoginPath = "/Identity/Account/Login";
    //     options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    //     options.SlidingExpiration = true;
    // });

    services.AddRazorPages();

}

var builder = WebApplication.CreateBuilder(args);


ConfigureServices(builder.Services, builder.Configuration, builder.Environment.IsDevelopment());

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

app.UseCors(options =>
{
    options.AllowAnyMethod().AllowAnyHeader();
    options.SetIsOriginAllowed((host) => true);
    options.AllowCredentials();
});

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
// app.Use(Middleware.LandingPageHandler.Main);

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    // endpoints.MapDefaultControllerRoute();
    endpoints.MapControllers();
    // endpoints.MapRazorPages();
    // endpoints.MapFallbackToFile("host.html").AllowAnonymous();
    endpoints.MapFallback(Middleware.LandingPageRedirector.Main);
});

app.Run();