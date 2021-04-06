using Microsoft.AspNetCore.Mvc;

namespace Vysotsky.API.Models
{
    public class SearchParameters
    {
        [FromQuery(Name = "q")] public string Query { get; init; } = "";
    }
}
