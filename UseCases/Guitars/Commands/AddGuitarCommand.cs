using MediatR;
using PaginationResultWebApi.Common;
using PaginationResultWebApi.Entities;
using PaginationResultWebApi.Services.Contracts;

namespace PaginationResultWebApi.UseCases.Guitars.Commands;

public class AddGuitarCommand : IRequest<ApiResponse>
{
    public string Name { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
}

public class AddGuitarCommandHandler(IGuitarService guitarService) : IRequestHandler<AddGuitarCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(AddGuitarCommand request, CancellationToken cancellationToken)
    {
        var guitar = await guitarService.Add(new Guitar()
        {
            Name = request.Name,
            Model = request.Model,
            Brand = request.Brand,
        });

        return new ApiResponse(success: true, message: null, data: guitar);
    }
}