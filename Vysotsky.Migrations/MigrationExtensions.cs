using System.Linq;
using FluentMigrator.Builders.Create;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Builders.Execute;

namespace Vysotsky.Migrations
{
    public static class MigrationExtensions
    {
        public static ICreateTableWithColumnSyntax Entity(this ICreateExpressionRoot create, string name) =>
            create.Table(name)
                .WithColumn("id")
                .AsInt64()
                .Identity()
                .PrimaryKey()
                .WithColumn("created_at")
                .AsDateTime();

        public static void CreateEnum(this IExecuteExpressionRoot execute, string name, params string[] values) =>
            execute.Sql($"CREATE TYPE {name} as ENUM ({string.Join(", ", values.Select(v => $"'{v}'"))});");

        public static void DropEnum(this IExecuteExpressionRoot execute, string name) =>
            execute.Sql($"DROP TYPE {name};");

        public static ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax WithReference(
            this ICreateTableWithColumnSyntax table, string name,
            string primaryTable) =>
            table.WithColumn(name)
                .AsInt64()
                .ForeignKey(primaryTable, "id");

        public static ICreateTableColumnOptionOrWithColumnSyntax AsJsonb(this ICreateTableColumnAsTypeSyntax column) =>
            column.AsCustom("jsonb");

        public static ICreateTableColumnOptionOrWithColumnSyntax AsEnum(this ICreateTableColumnAsTypeSyntax column,
            string enumName) =>
            column.AsCustom(enumName);
    }
}