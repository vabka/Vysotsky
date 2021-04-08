namespace Vysotsky.API.Controllers.Customers.Dto
{
    /// <summary>
    /// ФИО
    /// </summary>
    public record PersonName(string FirstName, string LastName, string? Patronymic = null);
}