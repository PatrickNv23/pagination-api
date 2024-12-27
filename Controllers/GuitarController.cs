using Microsoft.AspNetCore.Mvc;
using PaginationResultWebApi.Entities;
using PaginationResultWebApi.Services.Contracts;

namespace PaginationResultWebApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class GuitarController(IGuitarService guitarService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetGuitars()
    {
        var guitars = await guitarService.ListAll();
        return new ApiResponse(success: true, message: null, data: guitars);
    }

    [HttpGet("Pagination")]
    public async Task<ActionResult<ApiResponse>> GetGuitarsByPagination(int pageIndex = 1, int pageSize = 5)
    {
        var guitars = await guitarService.ListAllByPagination(pageIndex, pageSize);
        return new ApiResponse(success: true, message: null, data: guitars);
    }
}