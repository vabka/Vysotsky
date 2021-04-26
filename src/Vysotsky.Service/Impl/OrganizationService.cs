using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Vysotsky.Data;
using Vysotsky.Data.Entities;
using Vysotsky.Service.Interfaces;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Impl
{
    public class OrganizationService : IOrganizationService
    {
        private readonly VysotskyDataConnection dataConnection;

        public OrganizationService(VysotskyDataConnection dataConnection) => this.dataConnection = dataConnection;

        public async Task<Organization> CreateOrganizationAsync(string name, Room[] rooms)
        {
            await using var transaction = await dataConnection.BeginTransactionAsync();
            var organizationId = await dataConnection.Organizations.InsertWithInt64IdentityAsync(() =>
                new OrganizationRecord
                {
                    Name = name,
                });
            await dataConnection.Users.UpdateAsync(_ => new UserRecord
            {
                OrganizationId = organizationId
            });
            await transaction.CommitAsync();
            return new Organization
            {
                Id = organizationId,
                Name = name
            };
        }

        public async Task<Organization?> GetOrganizationByIdOrNullAsync(long organizationId) =>
            await dataConnection.Organizations
                .Select(o => new Organization
                {
                    Id = o.Id,
                    Name = o.Name
                })
                .FirstOrDefaultAsync(x => x.Id == organizationId);

        public async Task UpdateOrganization(Organization newOrganization) =>
            await dataConnection.Organizations
                .UpdateAsync(o => o.Id == newOrganization.Id, o =>
                    new OrganizationRecord
                    {
                        Name = newOrganization.Name
                    });

        public async Task<Organization[]> GetAllOrganizations() =>
            await dataConnection.Organizations
                .OrderBy(o => o.CreatedAt)
                .Select(o => new Organization
                {
                    Id = o.Id,
                    Name = o.Name
                })
                .ToArrayAsync();
    }
}
