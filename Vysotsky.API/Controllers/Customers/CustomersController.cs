using System;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Controllers.Rooms;
using Vysotsky.API.Infrastructure;
using Vysotsky.API.Models;

namespace Vysotsky.API.Controllers.Customers
{
    [Route(Resources.Customers)]
    public class CustomersController : ApiController
    {
        [HttpGet("{id:int}")]
        public ActionResult<ApiResponse<Customer>> GetCustomer(int id)
        {
            throw new NotImplementedException();
        }
    }

    public class Customer
    {
        public int Id { get; init; }
        public PersonName? Person { get; init; }
        public OrganizationName? Organization { get; init; }
        public Room[] Rooms { get; init; }
        public CustomerContact[] Contacts { get; init; }
    }

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