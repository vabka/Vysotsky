using Vysotsky.Data.Entities;
using Vysotsky.Service.Interfaces;

namespace Vysotsky.API.Infrastructure
{
    public static class CurrentUserExtensions
    {
        public static bool IsCustomer(this User currentUser) =>
            currentUser.Role is UserRole.OrganizationOwner or UserRole.OrganizationMember;

        public static bool CanReadOrganization(this User currentUser, long organizationId) =>
            currentUser switch
            {
                null                                                                                        => false,
                {Role: UserRole.SuperUser}                                                                  => true,
                {Role: UserRole.Supervisor}                                                                 => true,
                {Role: UserRole.Worker}                                                                     => true,
                {Role: UserRole.OrganizationMember, OrganizationId: var orgId} when orgId == organizationId => true,
                {Role: UserRole.OrganizationOwner, OrganizationId: var orgId} when orgId == organizationId  => true,
                _                                                                                           => false
            };

        public static bool CanWriteOrganization(this User currentUser, long organizationId) =>
            currentUser.CanReadOrganization(organizationId) &&
            currentUser.Role switch
            {
                UserRole.SuperUser          => true,
                UserRole.Supervisor         => true,
                UserRole.OrganizationOwner  => true,
                UserRole.Worker             => throw new System.NotImplementedException(),
                UserRole.OrganizationMember => throw new System.NotImplementedException(),
                _                           => false
            };


        public static bool IsWorker(this User currentUser) =>
            currentUser.Role is UserRole.Worker;

        public static bool CanCompleteIssue(this User currentUser, long workerId) =>
            currentUser switch
            {
                {Role: UserRole.Supervisor or UserRole.SuperUser} => true,
                {Role: UserRole.Worker, Id: var id}               => workerId == id,
                _                                                 => false
            };

        public static bool IsSupervisor(this User currentUser) =>
            currentUser.Role switch
            {
                UserRole.Supervisor         => true,
                UserRole.SuperUser          => true,
                UserRole.Worker             => throw new System.NotImplementedException(),
                UserRole.OrganizationOwner  => throw new System.NotImplementedException(),
                UserRole.OrganizationMember => throw new System.NotImplementedException(),
                _                           => false
            };

        public static bool IsSuperuser(this User currentUser) =>
            currentUser.Role == UserRole.SuperUser;

        public static bool CanEditCategories(this User currentUser) =>
            currentUser.Role switch
            {
                UserRole.Supervisor         => true,
                UserRole.SuperUser          => true,
                UserRole.Worker             => throw new System.NotImplementedException(),
                UserRole.OrganizationOwner  => throw new System.NotImplementedException(),
                UserRole.OrganizationMember => throw new System.NotImplementedException(),
                _                           => false
            };
    }
}
