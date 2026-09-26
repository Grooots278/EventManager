using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialAuthenticationNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_refresh_sessions_users_UserId",
                table: "refresh_sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_user_profiles_cities_CityId",
                table: "user_profiles");

            migrationBuilder.DropForeignKey(
                name: "FK_user_profiles_users_UserId",
                table: "user_profiles");

            migrationBuilder.RenameColumn(
                name: "Login",
                table: "users",
                newName: "login");

            migrationBuilder.RenameColumn(
                name: "UpdatedAtUtc",
                table: "users",
                newName: "updated_at_utc");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "users",
                newName: "password_hash");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "users",
                newName: "created_at_utc");

            migrationBuilder.RenameIndex(
                name: "IX_users_Login",
                table: "users",
                newName: "IX_users_login");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "user_profiles",
                newName: "role");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "user_profiles",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "UpdatedAtUtc",
                table: "user_profiles",
                newName: "updated_at_utc");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "user_profiles",
                newName: "last_name");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "user_profiles",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "user_profiles",
                newName: "first_name");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "user_profiles",
                newName: "created_at_utc");

            migrationBuilder.RenameColumn(
                name: "CityId",
                table: "user_profiles",
                newName: "city_id");

            migrationBuilder.RenameColumn(
                name: "BirthDate",
                table: "user_profiles",
                newName: "birth_date");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "user_profiles",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_user_profiles_Email",
                table: "user_profiles",
                newName: "IX_user_profiles_email");

            migrationBuilder.RenameIndex(
                name: "IX_user_profiles_CityId",
                table: "user_profiles",
                newName: "IX_user_profiles_city_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "refresh_sessions",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "TokenHash",
                table: "refresh_sessions",
                newName: "token_hash");

            migrationBuilder.RenameColumn(
                name: "RevokedAtUtc",
                table: "refresh_sessions",
                newName: "revoked_at_utc");

            migrationBuilder.RenameColumn(
                name: "ExpiresAtUtc",
                table: "refresh_sessions",
                newName: "expires_at_utc");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "refresh_sessions",
                newName: "created_at_utc");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_sessions_UserId",
                table: "refresh_sessions",
                newName: "IX_refresh_sessions_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_sessions_TokenHash",
                table: "refresh_sessions",
                newName: "IX_refresh_sessions_token_hash");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_sessions_ExpiresAtUtc",
                table: "refresh_sessions",
                newName: "IX_refresh_sessions_expires_at_utc");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "cities",
                newName: "name");

            migrationBuilder.RenameIndex(
                name: "IX_cities_Name",
                table: "cities",
                newName: "IX_cities_name");

            migrationBuilder.AddForeignKey(
                name: "FK_refresh_sessions_users_user_id",
                table: "refresh_sessions",
                column: "user_id",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_profiles_cities_city_id",
                table: "user_profiles",
                column: "city_id",
                principalTable: "cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_user_profiles_users_user_id",
                table: "user_profiles",
                column: "user_id",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_refresh_sessions_users_user_id",
                table: "refresh_sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_user_profiles_cities_city_id",
                table: "user_profiles");

            migrationBuilder.DropForeignKey(
                name: "FK_user_profiles_users_user_id",
                table: "user_profiles");

            migrationBuilder.RenameColumn(
                name: "login",
                table: "users",
                newName: "Login");

            migrationBuilder.RenameColumn(
                name: "updated_at_utc",
                table: "users",
                newName: "UpdatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "password_hash",
                table: "users",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "created_at_utc",
                table: "users",
                newName: "CreatedAtUtc");

            migrationBuilder.RenameIndex(
                name: "IX_users_login",
                table: "users",
                newName: "IX_users_Login");

            migrationBuilder.RenameColumn(
                name: "role",
                table: "user_profiles",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "user_profiles",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "updated_at_utc",
                table: "user_profiles",
                newName: "UpdatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "last_name",
                table: "user_profiles",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "user_profiles",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "first_name",
                table: "user_profiles",
                newName: "FirstName");

            migrationBuilder.RenameColumn(
                name: "created_at_utc",
                table: "user_profiles",
                newName: "CreatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "city_id",
                table: "user_profiles",
                newName: "CityId");

            migrationBuilder.RenameColumn(
                name: "birth_date",
                table: "user_profiles",
                newName: "BirthDate");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "user_profiles",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_user_profiles_email",
                table: "user_profiles",
                newName: "IX_user_profiles_Email");

            migrationBuilder.RenameIndex(
                name: "IX_user_profiles_city_id",
                table: "user_profiles",
                newName: "IX_user_profiles_CityId");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "refresh_sessions",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "token_hash",
                table: "refresh_sessions",
                newName: "TokenHash");

            migrationBuilder.RenameColumn(
                name: "revoked_at_utc",
                table: "refresh_sessions",
                newName: "RevokedAtUtc");

            migrationBuilder.RenameColumn(
                name: "expires_at_utc",
                table: "refresh_sessions",
                newName: "ExpiresAtUtc");

            migrationBuilder.RenameColumn(
                name: "created_at_utc",
                table: "refresh_sessions",
                newName: "CreatedAtUtc");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_sessions_user_id",
                table: "refresh_sessions",
                newName: "IX_refresh_sessions_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_sessions_token_hash",
                table: "refresh_sessions",
                newName: "IX_refresh_sessions_TokenHash");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_sessions_expires_at_utc",
                table: "refresh_sessions",
                newName: "IX_refresh_sessions_ExpiresAtUtc");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "cities",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_cities_name",
                table: "cities",
                newName: "IX_cities_Name");

            migrationBuilder.AddForeignKey(
                name: "FK_refresh_sessions_users_UserId",
                table: "refresh_sessions",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_profiles_cities_CityId",
                table: "user_profiles",
                column: "CityId",
                principalTable: "cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_user_profiles_users_UserId",
                table: "user_profiles",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
