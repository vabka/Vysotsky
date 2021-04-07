namespace Vysotsky.API.Controllers.Customers
{
    /// <summary>
    /// Контакт с клиентом
    /// </summary>
    public record CustomerContact(
        string Name,
        string Value,
        ContactType? KnownType = null);
}