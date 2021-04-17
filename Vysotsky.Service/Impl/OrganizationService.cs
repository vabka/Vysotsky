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
        private readonly VysotskyDataConnection _dataConnection;

        public OrganizationService(VysotskyDataConnection dataConnection) => _dataConnection = dataConnection;

        public async Task<Organization> CreateOrganizationAsync(string name, Room[] rooms)
        {
            await using var transaction = await _dataConnection.BeginTransactionAsync();
            var organizationId = await _dataConnection.Organizations.InsertWithInt64IdentityAsync(() =>
                new OrganizationRecord
                {
                    Name = name,
                });
            await _dataConnection.Users.UpdateAsync(_ => new UserRecord
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
            await _dataConnection.Organizations
                .Select(o => new Organization
                {
                    Id = o.Id,
                    Name = o.Name
                })
                .SingleOrDefaultAsync(x => x.Id == organizationId);

        public async Task UpdateOrganization(Organization newOrganization) =>
            await _dataConnection.Organizations
                .UpdateAsync(o => o.Id == newOrganization.Id, o =>
                    new OrganizationRecord
                    {
                        Name = newOrganization.Name
                    });

        public async Task<Organization[]> GetAllOrganizations() =>
            await _dataConnection.Organizations
                .OrderBy(o => o.CreatedAt)
                .Select(o => new Organization
                {
                    Id = o.Id,
                    Name = o.Name
                })
                .ToArrayAsync();
    }
}
