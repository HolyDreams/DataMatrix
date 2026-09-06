using DataMatrix.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataMatrix.Api.DAL;

/// <summary>
/// Контекст бд
/// </summary>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User, Role, int>(options)
{
    public DbSet<Code> Codes { get; set; }
    public DbSet<UserRoleMapping> UserRolesMapping { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.UserName).IsRequired();
            entity.Property(u => u.PasswordHash).IsRequired();
            
            entity.HasMany(u => u.Codes)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity<UserRoleMapping>(
                    j => j.HasOne(ur => ur.Role)
                        .WithMany()
                        .HasForeignKey(ur => ur.RoleId),
                    j => j.HasOne(ur => ur.User)
                        .WithMany()
                        .HasForeignKey(ur => ur.UserId),
                    j =>
                    {
                        j.HasKey(ur => new { ur.UserId, ur.RoleId });
                        j.ToTable("UserRolesMapping");
                    });
        });
        
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Name).IsRequired().HasMaxLength(50);
            
            entity.HasMany(r => r.Users)
                .WithMany(u => u.Roles)
                .UsingEntity<UserRoleMapping>(
                    j => j.HasOne(ur => ur.User)
                        .WithMany()
                        .HasForeignKey(ur => ur.UserId),
                    j => j.HasOne(ur => ur.Role)
                        .WithMany()
                        .HasForeignKey(ur => ur.RoleId),
                    j =>
                    {
                        j.HasKey(ur => new { ur.UserId, ur.RoleId });
                        j.ToTable("UserRolesMapping");
                    });
        });

        modelBuilder.Entity<Code>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Value).IsRequired().HasMaxLength(50);
            entity.Property(c => c.UserId).IsRequired();
            
            entity.HasOne(c => c.User)
                .WithMany(u => u.Codes)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<UserRoleMapping>(entity =>
        {
            entity.HasKey(ur => new { ur.UserId, ur.RoleId });

            entity.HasOne(ur => ur.User)
                .WithMany()
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ur => ur.Role)
                .WithMany()
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Ignore<IdentityPasskeyData>();
    }
}