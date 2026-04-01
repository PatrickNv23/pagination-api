using Google.GenAI;
using MediatR;
using ApiResponse = PaginationResultWebApi.Common.ApiResponse;

namespace PaginationResultWebApi.UseCases.Invoice.Queries
{
    public class GetRandomInvoiceTextQuery : IRequest<ApiResponse>
    {
    }

    public class GetRandomInvoiceTextQueryHandler(IConfiguration configuration) : IRequestHandler<GetRandomInvoiceTextQuery, ApiResponse>
    {
        public async Task<ApiResponse> Handle(GetRandomInvoiceTextQuery request, CancellationToken cancellationToken)
        {
            var apiResponse = new ApiResponse(success: false, null);

            string apiKey = configuration["Gemini:ApiKey"] ?? string.Empty;

            if (string.IsNullOrEmpty(apiKey))
            {
                apiResponse.Message = "API Key not found.";
                return apiResponse;
            }

            var client = new Client(apiKey: apiKey);

            var response = await client.Models.GenerateContentAsync(
                    model: "gemini-2.5-flash",
                    contents: "Dime los mejores ejercicios para fortalecer la espalda"
                );

            if (response?.Candidates == null | (response?.Candidates ?? []).Count == 0)
            {
                apiResponse.Message = "No response from Gemini API.";
                return apiResponse;
            }

            Console.WriteLine((response?.Candidates ?? [])[0].Content?.Parts?[0]?.Text ?? "Unknown");

            apiResponse.Success = true;
            apiResponse.Message = "Random text succesfully. ";
            apiResponse.Data = (response?.Candidates ?? [])[0].Content?.Parts?[0]?.Text ?? "Unknown";

            return apiResponse;
        }
    }
}
