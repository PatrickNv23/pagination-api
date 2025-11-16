using MediatR;
using PaginationResultWebApi.Common;
using PaginationResultWebApi.Services.Contracts;

namespace PaginationResultWebApi.UseCases.Guitars.Queries;

public record ListBestSuggestionsQuery(string SearchTerm) : IRequest<ApiResponse>;

public class ListBestSuggestionsQueryHandler(IGuitarService guitarService)
    : IRequestHandler<ListBestSuggestionsQuery, ApiResponse>
{
    public async Task<ApiResponse> Handle(ListBestSuggestionsQuery request, CancellationToken cancellationToken)
    {
        var guitars = await guitarService.ListAll();
        var searchItems = guitars
            .SelectMany(g => new[] { g.Name, g.Model, g.Brand })
            .Where(x => !string.IsNullOrEmpty(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
            
        var results = await FuzzySearchHelper.SearchAsync(request.SearchTerm, searchItems);
        return new ApiResponse(success: true, message: "Search suggestions executed succesfully", data: results);
    }
}