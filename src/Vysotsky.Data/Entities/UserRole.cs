using LinqToDB.Mapping;

namespace Vysotsky.Data.Entities
{
    public enum UserRole
    {
        [MapValue("SuperUser")] SuperUser,
        [MapValue("Supervisor")] Supervisor,
        [MapValue("Worker")] Worker,
        [MapValue("OrganizationOwner")] OrganizationOwner,
        [MapValue("OrganizationMember")] OrganizationMember
    }
}
