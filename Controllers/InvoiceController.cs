using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaginationResultWebApi.UseCases.Invoice.Commands;
using PaginationResultWebApi.UseCases.Invoice.Queries;
using ApiResponse = PaginationResultWebApi.Common.ApiResponse;

namespace PaginationResultWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class InvoiceController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [HttpGet("GeminiApi")]
    public async Task<ApiResponse> GetRandomText()
    {
        return await _mediator.Send(new GetRandomInvoiceTextQuery());
    }

    [HttpPost("ExtractInvoiceData")]
    public async Task<ApiResponse> ExtractInvoiceData(IFormFile file)
    {
        return await _mediator.Send(new ExtractInvoiceDataCommand
        {
            File = file
        });
    }
}
