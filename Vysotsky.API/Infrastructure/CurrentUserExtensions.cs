using Vysotsky.Data.Entities;

namespace Vysotsky.API.Infrastructure
{
    public static class CurrentUserExtensions
    {
        public static bool CanReadOrganization(this ICurrentUserProvider currentUserProvider, long organizationId)
        {
            var currentUser = currentUserProvider.CurrentUser;
            return currentUser switch
            {
                {Role: UserRole.SuperUser} => true,
                {Role: UserRole.Supervisor} => true,
                {Role: UserRole.Worker} => true,
                {Role: UserRole.OrganizationMember, OrganizationId: var orgId} when orgId == organizationId => true,
                {Role: UserRole.OrganizationOwner, OrganizationId: var orgId} when orgId == organizationId => true,
                _ => false
            };
        }

        public static bool CanWriteOrganization(this ICurrentUserProvider currentUserProvider, long organizationId)
        {
            var currentUser = currentUserProvider.CurrentUser;
            return currentUserProvider.CanReadOrganization(organizationId) &&
                   (currentUser?.Role == UserRole.SuperUser
                    || currentUser?.Role == UserRole.Supervisor
                    || currentUser?.Role == UserRole.OrganizationOwner
                   );
        }
    }
}