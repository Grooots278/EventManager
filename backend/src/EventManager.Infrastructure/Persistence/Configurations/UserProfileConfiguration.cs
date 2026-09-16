using EventManager.Domain.Cities;
using EventManager.Domain.Users;
using EventManager.Domain.Users.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventManager.Infrastructure.Persistence.Configurations;

public sealed class UserProfileConfiguration
    : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("user_profiles");

        builder.HasKey(x => x.UserId);

        builder.Property(x => x.FirstName)
            .HasConversion(
                x => x.Value,
                x => FirstName.Create(x)
            )
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasConversion(
                x => x.Value,
                x => LastName.Create(x)
            )
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasConversion(
                x => x.Value,
                x => Email.Create(x)
            )
            .HasMaxLength(254)
            .IsRequired();

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(x => x.Role)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasOne<City>()
            .WithMany()
            .HasForeignKey(x => x.CityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
