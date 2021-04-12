using System.Threading.Tasks;

namespace Vysotsky.Service.Interfaces
{
    public interface IOrganizationService
    {
        Task<Organization> CreateOrganization(string name);
        Task<Organization?> GetOrganizationByIdOrNull(long organizationId);
    }
    
    public class Organization
    {
        public long Id { get; init; }
        public string Name { get; init; } = null!;
    }
}