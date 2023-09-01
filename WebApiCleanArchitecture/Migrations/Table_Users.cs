using FluentMigrator;

namespace WebApiCleanArchitecture.Migrations;

[Migration(202308130926)]
public class Table_Users : Migration
{
    public override void Down()
    {
        Delete.Table("Users");
    }

    public override void Up()
    {
        Create.Table("Users")
        .WithColumn("Id").AsInt32().NotNullable().Identity().PrimaryKey()
        .WithColumn("Email").AsString(50).NotNullable()
        .WithColumn("Username").AsString(50).NotNullable()
        .WithColumn("Password").AsString(250).NotNullable()
        .WithColumn("CreatedAt").AsDateTime().Nullable()
        .WithColumn("UpdatedAt").AsDateTime().Nullable();
    }
}
