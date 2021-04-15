using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Data;
using Npgsql;
using Npgsql.NameTranslation;
using Npgsql.TypeHandlers;
using Npgsql.TypeMapping;
using NpgsqlTypes;
using Vysotsky.Data.Entities;

namespace Vysotsky.Data
{
    public class VysotskyDataConnection : DataConnection
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            Converters = { new JsonStringEnumConverter() },
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        static VysotskyDataConnection() =>
            NpgsqlConnection.GlobalTypeMapper
                .AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "jsonb",
                    NpgsqlDbType = NpgsqlDbType.Jsonb,
                    TypeHandlerFactory = new JsonbHandlerFactory(JsonSerializerOptions)
                }.Build())
                .MapEnum<UserRole>("user_role", new NpgsqlNullNameTranslator())
                .MapEnum<IssueStatus>("issue_status", new NpgsqlNullNameTranslator())
                .MapEnum<RoomStatus>("room_status", new NpgsqlNullNameTranslator());

        public VysotskyDataConnection(LinqToDbConnectionOptions<VysotskyDataConnection> options) : base(options)
        {
            this.InlineParameters = true;
            this.MappingSchema.SetConverter<string, UserContact[]?>(json =>
                JsonSerializer.Deserialize<UserContact[]>(json, JsonSerializerOptions));
            this.MappingSchema.SetConverter<string, Dictionary<string, string>?>(json =>
                JsonSerializer.Deserialize<Dictionary<string, string>?>(json, JsonSerializerOptions));
        }

        public ITable<UserRecord> Users => this.GetTable<UserRecord>();
        public ITable<BlockedTokenRecord> BlockedTokens => this.GetTable<BlockedTokenRecord>();
        public ITable<OrganizationRecord> Organizations => this.GetTable<OrganizationRecord>();
        public ITable<BuildingRecord> Buildings => this.GetTable<BuildingRecord>();
        public ITable<FloorRecord> Floors => this.GetTable<FloorRecord>();
        public ITable<RoomRecord> Rooms => this.GetTable<RoomRecord>();
        public ITable<ImageRecord> Images => this.GetTable<ImageRecord>();
        public ITable<IssueRecord> Issues => this.GetTable<IssueRecord>();
        public ITable<IssueImageRelation> IssueImages => this.GetTable<IssueImageRelation>();
        public ITable<IssueCommentRecord> IssueComments => this.GetTable<IssueCommentRecord>();
        public ITable<IssueCommentImageRelation> IssueCommentImages => this.GetTable<IssueCommentImageRelation>();
        public ITable<CategoryRecord> Categories => this.GetTable<CategoryRecord>();
        public ITable<AreaRecord> Areas => this.GetTable<AreaRecord>();
    }
}
