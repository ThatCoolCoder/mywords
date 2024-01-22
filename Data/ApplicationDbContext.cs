using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true); // prevent weird postgres complaints about dates
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("dbo");

        modelBuilder.Entity<ApplicationUser>(b => b.ToTable("aspnetuser"));
        modelBuilder.Entity<IdentityUserClaim<string>>(b => b.ToTable("aspnetuserclaim"));
        modelBuilder.Entity<IdentityUserLogin<string>>(b => b.ToTable("aspnetuserlogin"));
        modelBuilder.Entity<IdentityUserToken<string>>(b => b.ToTable("aspnetusertoken"));
        modelBuilder.Entity<IdentityRole>(b => b.ToTable("aspnetrole"));
        modelBuilder.Entity<IdentityRoleClaim<string>>(b => b.ToTable("aspnetroleclaim"));
        modelBuilder.Entity<IdentityUserRole<string>>(b => b.ToTable("aspnetuserrole"));

        // modelBuilder.Entity<LabelTerm>(b => b.ToTable("labelterm"));

        // modelBuilder.Entity<Term>()
        //     .HasMany(t => t.Labels)
        //     .WithMany(l => l.Terms)
        //     .UsingEntity<TermLabel>("termlabel",
        //     t => t.HasOne<Label>().WithMany().HasForeignKey("labelid"),
        //     l => l.HasOne<Term>().WithMany().HasForeignKey("termid"));
    }

    public virtual DbSet<ApplicationUser> ApplicationUser { get; set; } = null!;
    public virtual DbSet<TermSet> TermSet { get; set; } = null!;
    public virtual DbSet<Term> Term { get; set; } = null!;
    public virtual DbSet<Label> Label { get; set; } = null!;
    public virtual DbSet<LabelTerm> LabelTerm { get; set; } = null!;

    public ApplicationUser GetLoggedInUser(HttpContext context)
    {
        var user = ApplicationUser.FirstOrDefault(x => x.Email == context.User.Identity!.Name);
        if (user == null) throw new Exception("Attempted to get logged in user but user is not logged in (or no matching user exists)");
        return user;
    }
}