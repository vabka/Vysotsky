using System.Linq;
using FluentMigrator.Builders;
using FluentMigrator.Builders.Alter.Table;
using FluentMigrator.Builders.Create;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Builders.Execute;
using FluentMigrator.Infrastructure;

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

        public static ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax References(
            this ICreateTableColumnAsTypeSyntax column, string primary) =>
            column.AsInt64().ForeignKey(primary, "id");

        public static IAlterTableColumnOptionOrAddColumnOrAlterColumnOrForeignKeyCascadeSyntax References(this IAlterTableColumnAsTypeSyntax column, string primary) =>
            column.AsInt64().ForeignKey(primary, "id");

        public static TNext AsJsonb<TNext>(this IColumnTypeSyntax<TNext> column) where TNext : IFluentSyntax =>
            column.AsCustom("jsonb");

        public static TNext AsEnum<TNext>(this IColumnTypeSyntax<TNext> column,
            string enumName) where TNext : IFluentSyntax => column.AsCustom(enumName);
    }
}
