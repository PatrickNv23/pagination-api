namespace PaginationResultWebApi.Entities;

public class GeneralRateLimitPolicies
{
    public const string RateLimitPolicies = "RateLimitPolicies";
    public string? FixedPolicy { get; set; }
    public string? SlidingPolicy { get; set; }
    public string? TokenPolicy { get; set; }
    public string? ConcurrencyPolicy { get; set; }
}