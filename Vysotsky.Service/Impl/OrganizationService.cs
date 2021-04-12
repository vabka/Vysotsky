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

        public OrganizationService(VysotskyDataConnection dataConnection)
        {
            _dataConnection = dataConnection;
        }

        public async Task<Organization> CreateOrganization(User owner, string name)
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

        public Task<Organization?> GetOrganizationByIdOrNull(long organizationId) =>
            _dataConnection.Organizations
                .Select(o => new Organization
                {
                    Id = o.Id,
                    Name = o.Name
                })
                .SingleOrDefaultAsync(x => x.Id == organizationId);

        public Task UpdateOrganization(Organization newOrganization)
        {
            throw new System.NotImplementedException();
        }

        public Task<Organization[]> GetAllOrganizations() =>
            _dataConnection.Organizations
                .OrderBy(o => o.CreatedAt)
                .Select(o => new Organization
                {
                    Id = o.Id,
                    Name = o.Name
                })
                .ToArrayAsync();
    }
}