
using FluentMigrator;

namespace WebApiCleanArchitecture.Migrations;

[Migration(202307241633)]
public class Table_Product : Migration
{
    public override void Down()
    {
        Delete.Table("Products");
    }

    public override void Up()
    {
        Create.Table("Products")
        .WithColumn("Id").AsInt32().NotNullable().Identity().PrimaryKey()
        .WithColumn("Name").AsString(50).NotNullable()
        .WithColumn("Price").AsString(50).NotNullable()
        .WithColumn("CreatedAt").AsDateTime().NotNullable()
        .WithColumn("UpdatedAt").AsDateTime().Nullable();
    }
}
