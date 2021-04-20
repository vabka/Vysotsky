using FluentMigrator;

namespace Vysotsky.Migrations.Migrations
{
    [CustomMigration("Remove subcategories", 2021, 04, 20, 16, 30, BreakingChange = false)]
    public class RemoveSubCategories : Migration
    {
        public override void Up()
        {
            Execute.Sql("UPDATE issue SET category_id = NULL WHERE category_id IS NOT NULL;");
            Delete.FromTable("category").AllRows();
            Alter.Table("category").AddColumn("image_id").References("image").Nullable();
            Delete.Column("area_id").FromTable("category");
            Execute.Sql("INSERT INTO category SELECT * FROM area;");
            Execute.Sql("UPDATE issue SET category_id = area_id WHERE category_id IS NULL");
            Alter.Table("issue").AlterColumn("category_id").References("category").NotNullable();
            Delete.Column("area_id").FromTable("issue");
            Delete.Table("area");
            Alter.Table("category").AlterColumn("image_id").References("image").NotNullable();
        }

        public override void Down() => throw new System.NotImplementedException();
    }
}
