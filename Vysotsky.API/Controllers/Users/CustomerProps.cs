using System;
using Vysotsky.API.Controllers.Customers;

namespace Vysotsky.API.Controllers.Users
{
    public class CustomerProps
    {
        public PersonName? Person { get; init; }
        public OrganizationName? Organization { get; init; }
        public int[] Rooms { get; init; } = Array.Empty<int>();
        public CustomerContact[] Contacts { get; init; } = Array.Empty<CustomerContact>();
    }
}