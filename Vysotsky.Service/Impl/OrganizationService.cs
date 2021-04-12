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
            var organizationId = await _dataConnection.Organizations.InsertWithInt64IdentityAsync(() =>
                new OrganizationRecord
                {
                    Name = name,
                    OwnerId = owner.Id
                });
            
            return new Organization
            {
                Id = organizationId,
                Name = name
            };
        }

        public Task<Organization?> GetOrganizationByIdOrNull(long organizationId)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateOrganization(Organization newOrganization)
        {
            throw new System.NotImplementedException();
        }

        public Task<Organization[]> GetAllOrganizations()
        {
            throw new System.NotImplementedException();
        }

        public Task<FullBuilding[]> GetOrganizationBuildings(Organization organization)
        {
            throw new System.NotImplementedException();
        }
    }
}