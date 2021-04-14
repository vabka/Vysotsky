using System.Threading.Tasks;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Interfaces
{
    public interface IOrganizationService
    {
        Task<Organization> CreateOrganizationAsync(string name, Room[] rooms);
        Task<Organization?> GetOrganizationByIdOrNullAsync(long organizationId);
        Task UpdateOrganization(Organization newOrganization);
        Task<Organization[]> GetAllOrganizations();
    }
}
