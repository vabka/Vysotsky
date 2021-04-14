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
            Converters = {new JsonStringEnumConverter()},
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        static VysotskyDataConnection()
        {
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
        }

        public VysotskyDataConnection(LinqToDbConnectionOptions<VysotskyDataConnection> options) : base(options)
        {
            InlineParameters = true;
            MappingSchema.SetConverter<string, UserContact[]?>(json =>
                JsonSerializer.Deserialize<UserContact[]>(json, JsonSerializerOptions));
            MappingSchema.SetConverter<string, Dictionary<string, string>?>(json =>
                JsonSerializer.Deserialize<Dictionary<string, string>?>(json, JsonSerializerOptions));
        }

        public ITable<UserRecord> Users => GetTable<UserRecord>();
        public ITable<BlockedTokenRecord> BlockedTokens => GetTable<BlockedTokenRecord>();
        public ITable<OrganizationRecord> Organizations => GetTable<OrganizationRecord>();
        public ITable<BuildingRecord> Buildings => GetTable<BuildingRecord>();
        public ITable<FloorRecord> Floors => GetTable<FloorRecord>();
        public ITable<RoomRecord> Rooms => GetTable<RoomRecord>();
        public ITable<ImageRecord> Images => GetTable<ImageRecord>();
        public ITable<IssueRecord> Issues => GetTable<IssueRecord>();
        public ITable<IssueImageRelation> IssueImages => GetTable<IssueImageRelation>();
        public ITable<IssueCommentRecord> IssueComments => GetTable<IssueCommentRecord>();
        public ITable<IssueCommentImageRelation> IssueCommentImages => GetTable<IssueCommentImageRelation>();
        public ITable<CategoryRecord> Categories => GetTable<CategoryRecord>();
        public ITable<AreaRecord> Areas => GetTable<AreaRecord>();
    }
}