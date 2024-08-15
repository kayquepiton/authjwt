using Ca.Backend.Test.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ca.Backend.Test.Infrastructure.Data.Configurations;

public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .HasMaxLength(100);

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Property(u => u.RefreshToken)
            .HasMaxLength(200);

        builder.Property(u => u.ExpirationRefreshToken)
            .IsRequired();

        // Configuração da relação muitos-para-muitos com RoleEntity
        builder.HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity<Dictionary<string, object>>(
                "UserRole",
                j => j.HasOne<RoleEntity>().WithMany().HasForeignKey("RoleId"),
                j => j.HasOne<UserEntity>().WithMany().HasForeignKey("UserId"));
    }
}

public class RoleEntityConfiguration : IEntityTypeConfiguration<RoleEntity>
{
    public void Configure(EntityTypeBuilder<RoleEntity> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.Description)
            .HasMaxLength(200);
        
        // Configuração da relação muitos-para-muitos com UserEntity
        builder.HasMany(r => r.Users)
            .WithMany(u => u.Roles)
            .UsingEntity<Dictionary<string, object>>(
                "UserRole",
                j => j.HasOne<UserEntity>().WithMany().HasForeignKey("UserId"),
                j => j.HasOne<RoleEntity>().WithMany().HasForeignKey("RoleId"));
    }
}
