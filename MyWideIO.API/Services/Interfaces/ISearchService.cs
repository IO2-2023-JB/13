using MyWideIO.API.Models.Dto_Models;
using MyWideIO.API.Models.Enums;

namespace MyWideIO.API.Services.Interfaces
{
    public interface ISearchService
    {
        public Task<SearchResultsDto> GetSearchResultsAsync(string query, SortingTypesEnum sortingCriteria, SortingDirectionsEnum sortingType, DateTime? beginDate, DateTime? endDate);
    }
}
