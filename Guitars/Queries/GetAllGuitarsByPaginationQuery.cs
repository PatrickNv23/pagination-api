using MediatR;
using PaginationResultWebApi.Entities;
using PaginationResultWebApi.Services.Contracts;

namespace PaginationResultWebApi.Guitars.Queries;

public class GetAllGuitarsByPaginationQuery : IRequest<ApiResponse>
{
    public int pageIndex = 1;
    public int pageSize = 5;
}

public class GetAllGuitarsByPaginationHandler(IGuitarService guitarService) : IRequestHandler<GetAllGuitarsByPaginationQuery, ApiResponse>
{
    public async Task<ApiResponse> Handle(GetAllGuitarsByPaginationQuery request, CancellationToken cancellationToken)
    {
        var guitars = await guitarService.ListAllByPagination(request.pageIndex, request.pageSize);
        return new ApiResponse(success: true, message: null, data: guitars);
    }
}