using Vysotsky.Data.Entities;
using Vysotsky.Service.Interfaces;
using Vysotsky.Service.Types;

namespace Vysotsky.API.Infrastructure
{
    public static class CurrentUserExtensions
    {
        public static bool IsCustomer(this User currentUser) =>
            currentUser.Role is UserRole.OrganizationOwner or UserRole.OrganizationMember;

        public static bool CanReadOrganization(this User currentUser, long organizationId) =>
            currentUser.Role is UserRole.SuperUser or UserRole.Supervisor or UserRole.Worker ||
            (currentUser is
             {
                 Role: UserRole.OrganizationMember or UserRole.OrganizationOwner,
                 OrganizationId: var orgId
             } &&
             organizationId == orgId);

        public static bool CanWriteOrganization(this User currentUser, long organizationId) =>
            currentUser.CanReadOrganization(organizationId) &&
            currentUser.Role switch
            {
                UserRole.SuperUser         => true,
                UserRole.Supervisor        => true,
                UserRole.OrganizationOwner => true,
                _                          => false
            };


        public static bool IsWorker(this User currentUser) =>
            currentUser.Role is UserRole.Worker;

        public static bool CanCompleteIssue(this User currentUser, Issue issue) =>
            currentUser.IsSupervisor() || (currentUser is
            {
                Role: UserRole.Worker, Id: var id
            } && issue.WorkerId == id);

        public static bool IsSupervisor(this User currentUser) =>
            currentUser.Role is UserRole.Supervisor or UserRole.SuperUser;

        public static bool IsSuperuser(this User currentUser) =>
            currentUser.Role == UserRole.SuperUser;

        public static bool CanEditCategories(this User currentUser) =>
            currentUser.IsSupervisor();
    }
}
