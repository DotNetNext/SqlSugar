using System;
using SqlSugar.Migrations;

namespace Examples.Migrations
{
    [Migration(20251126002, "Add Roles table and UserRoles junction table")]
    public class AddUserRoles_20251126002 : Migration
    {
        public override void Up(SchemaBuilder schema)
        {
            schema.CreateTable("Roles", table =>
            {
                table.Int("Id").PrimaryKey().AutoIncrement();
                table.String("Name", 50).NotNull();
                table.String("Description", 255);
            });

            schema.CreateTable("UserRoles", table =>
            {
                table.Int("UserId").NotNull();
                table.Int("RoleId").NotNull();
                table.DateTime("AssignedAt").NotNull().Default("GETDATE()");
            });

            schema.Sql("ALTER TABLE UserRoles ADD CONSTRAINT FK_UserRoles_Users FOREIGN KEY (UserId) REFERENCES Users(Id)");
            schema.Sql("ALTER TABLE UserRoles ADD CONSTRAINT FK_UserRoles_Roles FOREIGN KEY (RoleId) REFERENCES Roles(Id)");
        }

        public override void Down(SchemaBuilder schema)
        {
            schema.DropTable("UserRoles");
            schema.DropTable("Roles");
        }
    }
}
