using EventManager.Domain.Users;
using EventManager.Domain.Users.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventManager.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration
    : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.Property(x => x.Login)
            .HasColumnName("login")
            .HasConversion(
                login => login.Value,
                value => Login.Create(value)
            )
            .HasMaxLength(32)
            .IsRequired();

        builder.HasIndex(x => x.Login)
            .IsUnique();

        builder.Property(x => x.PasswordHash)
            .HasColumnName("password_hash")
            .HasConversion(
                hash => hash.Value,
                value => PasswordHash.FromHash(value)
            )
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc");

        builder.HasOne(x => x.Profile)
            .WithOne()
            .HasForeignKey<UserProfile>(
                x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
