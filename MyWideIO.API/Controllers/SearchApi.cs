using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using WideIO.API.Attributes;
using WideIO.API.Models;

namespace MyWideIO.API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class SearchApiController : ControllerBase
    {
        /// <summary>
        /// Get search results
        /// </summary>
        /// <param name="query">Search query</param>
        /// <param name="sortingCriterion">Sorting criterion</param>
        /// <param name="sortingType">Sort direction</param>
        /// <param name="beginDate">Begin date filtering</param>
        /// <param name="endDate">End date filtering</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ValidateModelState]
        [SwaggerOperation("GetSearchResults")]
        [SwaggerResponse(statusCode: 200, type: typeof(SearchResultsDto), description: "OK")]
        public virtual IActionResult GetSearchResults([FromQuery(Name = "query")][Required()] string query, [FromQuery(Name = "sortingCriterion")][Required()] SortingTypes sortingCriterion, [FromQuery(Name = "sortingType")][Required()] SortingDirections sortingType, [FromQuery(Name = "beginDate")] DateTime? beginDate, [FromQuery(Name = "endDate")] DateTime? endDate)
        {
            throw new NotImplementedException();
        }
    }
}
