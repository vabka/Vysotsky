using System.Threading.Tasks;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Interfaces
{
    public interface IOrganizationService
    {
        Task<Organization> CreateOrganization(User owner, string name);
        Task<Organization?> GetOrganizationByIdOrNull(long organizationId);
        Task UpdateOrganization(Organization newOrganization);
        Task<Organization[]> GetAllOrganizations();
    }
}