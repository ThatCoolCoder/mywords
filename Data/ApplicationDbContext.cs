using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {

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
    }

    public virtual DbSet<ApplicationUser> ApplicationUser { get; set; } = null!;
    public virtual DbSet<TermSet> TermSet { get; set; } = null!;
    public virtual DbSet<Term> Term { get; set; } = null!;
    public virtual DbSet<Label> Label { get; set; } = null!;
    public virtual DbSet<TermLabel> TermLabel { get; set; } = null!;
}