using Vysotsky.Data.Entities;

namespace Vysotsky.API.Infrastructure
{
    public static class CurrentUserExtensions
    {
        public static bool CanReadOrganization(this ICurrentUserProvider currentUserProvider, long organizationId) =>
            currentUserProvider.CurrentUser switch
            {
                null => false,
                { Role: UserRole.SuperUser } => true,
                { Role: UserRole.Supervisor } => true,
                { Role: UserRole.Worker } => true,
                { Role: UserRole.OrganizationMember, OrganizationId: var orgId } when orgId == organizationId => true,
                { Role: UserRole.OrganizationOwner, OrganizationId: var orgId } when orgId == organizationId => true,
                _ => false
            };

        public static bool CanWriteOrganization(this ICurrentUserProvider currentUserProvider, long organizationId) =>
            currentUserProvider.CanReadOrganization(organizationId) &&
            currentUserProvider.CurrentUser.Role switch
            {
                UserRole.SuperUser => true,
                UserRole.Supervisor => true,
                UserRole.OrganizationOwner => true,
                UserRole.Worker => throw new System.NotImplementedException(),
                UserRole.OrganizationMember => throw new System.NotImplementedException(),
                _ => false
            };


        public static bool IsSupervisor(this ICurrentUserProvider currentUserProvider) =>
            currentUserProvider.CurrentUser.Role switch
            {
                UserRole.Supervisor => true,
                UserRole.SuperUser => true,
                UserRole.Worker => throw new System.NotImplementedException(),
                UserRole.OrganizationOwner => throw new System.NotImplementedException(),
                UserRole.OrganizationMember => throw new System.NotImplementedException(),
                _ => false
            };

        public static bool CanEditCategories(this ICurrentUserProvider currentUserProvider) =>
            currentUserProvider.CurrentUser.Role switch
            {
                UserRole.Supervisor => true,
                UserRole.SuperUser => true,
                UserRole.Worker => throw new System.NotImplementedException(),
                UserRole.OrganizationOwner => throw new System.NotImplementedException(),
                UserRole.OrganizationMember => throw new System.NotImplementedException(),
                _ => false
            };
    }
}
