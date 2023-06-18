using Microsoft.AspNetCore.Mvc;
using MyWideIO.API.Models.Dto_Models;
using MyWideIO.API.Models.Enums;
using MyWideIO.API.Services.Interfaces;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using WideIO.API.Attributes;

namespace MyWideIO.API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("search")]
    public class SearchApiController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchApiController(ISearchService searchService)
        {
            _searchService = searchService;
        }

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
        public async Task<IActionResult> GetSearchResults([FromQuery(Name = "query")][Required()] string query, [FromQuery(Name = "sortingCriterion")][Required()] SortingTypesEnum sortingCriterion, [FromQuery(Name = "sortingType")][Required()] SortingDirectionsEnum sortingType, [FromQuery(Name = "beginDate")] DateTime? beginDate, [FromQuery(Name = "endDate")] DateTime? endDate)
        {
            return Ok(await _searchService.GetSearchResultsAsync(query, sortingCriterion, sortingType, beginDate, endDate)); 
        }
    }
}
