using System;
using SqlSugar.Migrations;

namespace Examples.Migrations
{
    [Migration(20251126001, "Create Users table")]
    public class CreateUsersTable_20251126001 : Migration
    {
        public override void Up(SchemaBuilder schema)
        {
            schema.CreateTable("Users", table =>
            {
                table.Int("Id").PrimaryKey().AutoIncrement();
                table.String("Username", 50).NotNull();
                table.String("Email", 255).NotNull();
                table.String("PasswordHash", 255).NotNull();
                table.DateTime("CreatedAt").NotNull().Default("GETDATE()");
                table.DateTime("UpdatedAt").NotNull().Default("GETDATE()");
                table.Bool("IsActive").NotNull().Default("1");
            });

            schema.CreateUniqueIndex("Users", "IX_Users_Email", "Email");
            schema.CreateIndex("Users", "IX_Users_Username", "Username");
        }

        public override void Down(SchemaBuilder schema)
        {
            schema.DropTable("Users");
        }
    }
}
