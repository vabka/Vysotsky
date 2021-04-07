namespace Vysotsky.API.Controllers.Users
{
    public class UserProperties
    {
        public AuthProps Auth { get; init; }
        public CustomerProps? Customer { get; init; }
        public EmployeeProprs? Employee { get; init; }
        public SupervisorProps? Supervisor { get; init; }
    }
}