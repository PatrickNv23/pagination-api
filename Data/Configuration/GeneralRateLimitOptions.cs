namespace PaginationResultWebApi.Data.Configuration;
public class GeneralRateLimitOptions
{
    public const string RateLimiting = "RateLimiting";
    // public int PermitLimit { get; set; } = 2;
    public int PermitLimit { get; set; }
    public int Window { get; set; }
    public int QueueLimit { get; set; }
    
    public int SegmentsPerWindow { get; set; }
    public int TokenLimit { get; set; }
    public int TokensPerPeriod { get; set; }
    public int ReplenishmentPeriod { get; set; }
    public int PartitionedPermitLimit { get; set; }
    public int GlobalPermitLimit { get; set; }
}