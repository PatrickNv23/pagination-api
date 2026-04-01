using Google.GenAI;
using Google.GenAI.Types;
using MediatR;
using PaginationResultWebApi.UseCases.Invoice.Dtos;
using System.Text.Json;
using ApiResponse = PaginationResultWebApi.Common.ApiResponse;

namespace PaginationResultWebApi.UseCases.Invoice.Commands;

public class ExtractInvoiceDataCommand : IRequest<ApiResponse>
{
    public IFormFile File { get; set; }
}

public class ExtractInvoiceDataCommandHandler(IConfiguration configuration) : IRequestHandler<ExtractInvoiceDataCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(ExtractInvoiceDataCommand request, CancellationToken cancellationToken)
    {
        var apiResponse = new ApiResponse(success: false, null);
        var file = request.File;

        if (file == null || file.Length == 0) throw new Exception("Please upload a valid image. ");

        try
        {
            string apiKey = configuration["Gemini:ApiKey"] ?? string.Empty;
            var client = new Client(apiKey: apiKey);

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            string prompt = @"Analyze this invoice image and extract the data into a JSON format.
                          The invoice is in Spanish, but map the fields to these English keys:
                          {
                            ""CompanyName"": ""(found in '[Nombre de la compañía]')"",
                            ""StreetAddress"": ""(found in '[Dirección de la calle]')"",
                            ""City"": ""(found in '[Ciudad, Estado]')"",
                            ""PhoneNumber"": ""(found in 'Teléfono')"",
                            ""CustomerId"": ""(found in 'IDENTIFICACIÓN DEL CLIENTE')"",
                            ""Items"": [
                              { 
                                ""Description"": ""(from 'DESCRIPCIÓN')"", 
                                ""Quantity"": ""(from 'CANT')"", 
                                ""UnitPrice"": ""(from 'PRECIO UNITARIO')"", 
                                ""TotalAmount"": ""(from 'MONTO')"" 
                              }
                            ]
                          }
                          Return ONLY the JSON object. No markdown, no triple backticks.";

            var response = await client.Models.GenerateContentAsync(
                   model: "gemini-2.5-flash",
                   contents: new List<Content>
                   {
                           new Content {
                               Parts = new List<Part>
                               {
                                    new Part { Text = prompt },
                                    new Part { InlineData = new Blob { MimeType = file.ContentType, Data = ms.ToArray() } }
                               }
                           }
                   }
                );


            string rawJson = response?.Candidates?[0]?.Content?.Parts?[0]?.Text ?? "";
            rawJson = rawJson.Replace("```json", "").Replace("```", "").Trim();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var invoice = JsonSerializer.Deserialize<InvoiceDto>(rawJson, options);

            apiResponse.Success = true;
            apiResponse.Message = "extracted data succesfully. ";
            apiResponse.Data = invoice;
        }
        catch (Exception ex)
        {
            apiResponse.Message = ex.Message;
        }

        return apiResponse;
    }
}

