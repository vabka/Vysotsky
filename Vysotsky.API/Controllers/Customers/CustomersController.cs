using System;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Controllers.Rooms;
using Vysotsky.API.Models;

namespace Vysotsky.API.Controllers.Customers
{
    [Route("/api/customers")]
    public class CustomersController : ApiController
    {
        [HttpGet("{id:int}")]
        public ActionResult<ApiResponse<Customer>> GetCustomer(int id)
        {
            throw new NotImplementedException();
        }
    }

    public record Customer(
        int Id,
        PersonName? Person,
        OrganizationName? Organization,
        Room[] Rooms,
        CustomerContact[] Contacts);

    public record CustomerContact(
        string Name,
        string Value,
        ContactType? KnownType = null);

    public enum ContactType
    {
        Phone,
        Email,
        Whatsapp,
        Telegram,
        Viber
    }

    public record OrganizationName(string Name);

    public record PersonName(string FirstName, string LastName, string? Patronymic = null);
}
