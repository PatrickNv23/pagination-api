using MediatR;
using PaginationResultWebApi.Common;
using PaginationResultWebApi.Services.Contracts;

namespace PaginationResultWebApi.UseCases.Guitars.Queries;

public record ListSearchSuggestionsQuery(string SearchTerm) : IRequest<ApiResponse>;

public class ListSearchSuggestionsQueryHandler(IGuitarService guitarService)
    : IRequestHandler<ListSearchSuggestionsQuery, ApiResponse>
{
    public async Task<ApiResponse> Handle(ListSearchSuggestionsQuery request, CancellationToken cancellationToken)
    {
        var guitars = await guitarService.ListSearchSuggestions(request.SearchTerm);
        return new ApiResponse(success: true, message: "Search suggestions executed succesfully", data: guitars);
    }
}