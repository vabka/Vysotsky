namespace Vysotsky.API.Controllers.Customers.Dto
{
    /// <summary>
    /// Контакт с клиентом
    /// </summary>
    public record CustomerContact(
        string Name,
        string Value,
        ContactType? KnownType = null);
}