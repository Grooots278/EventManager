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
            .HasConversion(
                login => login.Value,
                value => Login.Create(value)
            )
            .HasMaxLength(32)
            .IsRequired();

        builder.HasIndex(x => x.Login)
            .IsUnique();

        builder.Property(x => x.PasswordHash)
            .HasConversion(
                hash => hash.Value,
                value => PasswordHash.FromHash(value)
            )
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc);

        builder.HasOne(x => x.Profile)
            .WithOne()
            .HasForeignKey<UserProfile>(
                x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
