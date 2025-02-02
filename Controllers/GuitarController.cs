using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PaginationResultWebApi.Common;
using PaginationResultWebApi.UseCases.Guitars.Commands;
using PaginationResultWebApi.Guitars.Queries;
using PaginationResultWebApi.Transport;
using PaginationResultWebApi.UseCases.Guitars.Queries;

namespace PaginationResultWebApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
[EnableRateLimiting("fixed")]
public class GuitarController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
     
    [HttpGet]
    // [Authorize]
    public async Task<ActionResult<ApiResponse>> GetGuitars()
    {
        return await _mediator.Send(new GetAllGuitarsQuery());
    }

    [HttpGet("Pagination")]
    [EnableRateLimiting("concurrency")]
    public async Task<ActionResult<ApiResponse>> GetGuitarsByPagination(int pageIndex = 1, int pageSize = 5)
    {
        return await _mediator.Send(
            new GetAllGuitarsByPaginationQuery(){ pageIndex = pageIndex, pageSize = pageSize }
            );
    }
    
    [HttpPost()]
    [DisableRateLimiting]
    public async Task<ActionResult<ApiResponse>> AddGuitar(AddGuitarRequestDto addGuitarRequestDto)
    {
        return await _mediator.Send(
            new AddGuitarCommand()
            {
                Name = addGuitarRequestDto.Name,
                Model = addGuitarRequestDto.Model,
                Brand = addGuitarRequestDto.Brand,
            }
        );
    }
}