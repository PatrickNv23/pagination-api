using MediatR;
using PaginationResultWebApi.Entities;
using PaginationResultWebApi.Services.Contracts;

namespace PaginationResultWebApi.Guitars.Queries;

public class GetAllGuitarsQuery : IRequest<ApiResponse>
{
    
}

public class GetAllGuitarsHandler(IGuitarService guitarService) : IRequestHandler<GetAllGuitarsQuery, ApiResponse>
{
    
    public async Task<ApiResponse> Handle(GetAllGuitarsQuery request, CancellationToken cancellationToken)
    {
        var guitars = await guitarService.ListAll();
        return new ApiResponse(success: true, message: "ListAll guitars response", data: guitars);
    }
}