using Microsoft.AspNetCore.Mvc;

namespace Vysotsky.API.Controllers.Common
{
    public class SearchParameters
    {
        /// <summary>
        /// Поисковой запрос
        /// </summary>
        [FromQuery(Name = "q")] public string Query { get; init; } = "";
    }
}
