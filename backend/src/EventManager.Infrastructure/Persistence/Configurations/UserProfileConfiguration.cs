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

        builder.Property(x => x.UserId)
            .HasColumnName("user_id");

        builder.Property(x => x.FirstName)
            .HasColumnName("first_name")
            .HasConversion(
                x => x.Value,
                x => FirstName.Create(x)
            )
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasColumnName("last_name")
            .HasConversion(
                x => x.Value,
                x => LastName.Create(x)
            )
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasColumnName("email")
            .HasConversion(
                x => x.Value,
                x => Email.Create(x)
            )
            .HasMaxLength(254)
            .IsRequired();

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(x => x.BirthDate)
            .HasColumnName("birth_date")
            .HasColumnType("date");

        builder.Property(x => x.Role)
            .HasColumnName("role")
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc");

        builder.Property(x => x.CityId)
            .HasColumnName("city_id");

        builder.HasOne<City>()
            .WithMany()
            .HasForeignKey(x => x.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.CityId);
    }
}
