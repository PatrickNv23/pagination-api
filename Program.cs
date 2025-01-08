using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using PaginationResultWebApi.Data;
using PaginationResultWebApi.Entities;
using PaginationResultWebApi.Repositories;
using PaginationResultWebApi.Repositories.Contracts;
using PaginationResultWebApi.Services;
using PaginationResultWebApi.Services.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddScoped<IGuitarService, GuitarService>();
builder.Services.AddScoped<IGuitarRepository, GuitarRepository>();

// MediatR
builder.Services.AddMediatR(
    cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly)
    );

builder.Services.Configure<GeneralRateLimitOptions>(builder.Configuration.GetSection(GeneralRateLimitOptions.RateLimiting));
builder.Services.Configure<GeneralRateLimitPolicies>(builder.Configuration.GetSection(GeneralRateLimitPolicies.RateLimitPolicies));

var generalRateLimitOptions = new GeneralRateLimitOptions();
var generalRateLimitPolicies = new GeneralRateLimitPolicies();

builder.Configuration.GetSection(GeneralRateLimitOptions.RateLimiting).Bind(generalRateLimitOptions);
builder.Configuration.GetSection(GeneralRateLimitPolicies.RateLimitPolicies).Bind(generalRateLimitPolicies);

builder.Services.AddRateLimiter(options =>
{
    //options.RejectionStatusCode = 429;
    // error mas especifico
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            await context.HttpContext.Response.WriteAsync(
                $"Too many requests. Please try again later {retryAfter.TotalSeconds} seconds. ",
                cancellationToken: token
            );
        }
        else
        {
            await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later. ",
                cancellationToken: token
            );
        }
    };
    
    options.AddFixedWindowLimiter(generalRateLimitPolicies.FixedPolicy ?? throw new InvalidOperationException(), opt =>
    {
        opt.PermitLimit = generalRateLimitOptions.PermitLimit;
        opt.Window = TimeSpan.FromSeconds(generalRateLimitOptions.Window);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        // options.QueueLimit = generalRateLimitOptions.QueueLimit;
    });
    
    options.AddSlidingWindowLimiter(generalRateLimitPolicies.SlidingPolicy ?? throw new InvalidOperationException(), opt =>
    {
        opt.PermitLimit = generalRateLimitOptions.PermitLimit;
        opt.Window = TimeSpan.FromSeconds(generalRateLimitOptions.Window);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.SegmentsPerWindow = generalRateLimitOptions.SegmentsPerWindow;
    });
    
    options.AddTokenBucketLimiter(generalRateLimitPolicies.TokenPolicy ?? throw new InvalidOperationException(), opt =>
    {
        opt.TokenLimit = generalRateLimitOptions.TokenLimit;
        opt.ReplenishmentPeriod = TimeSpan.FromSeconds(generalRateLimitOptions.ReplenishmentPeriod);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.TokensPerPeriod = generalRateLimitOptions.TokensPerPeriod;
    });
    
    options.AddConcurrencyLimiter(generalRateLimitPolicies.ConcurrencyPolicy ?? throw new InvalidOperationException(), opt =>
    {
        opt.PermitLimit = generalRateLimitOptions.PermitLimit;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
    
    // RATE LIMIT GLOBAL SIMPLE
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        return RateLimitPartition.GetFixedWindowLimiter("Global", partition =>
            new FixedWindowRateLimiterOptions
            {
                PermitLimit = generalRateLimitOptions.GlobalPermitLimit,
                Window = TimeSpan.FromSeconds(generalRateLimitOptions.Window),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            }
        );
    });
    
    // RATE LIMITING GLOBAL COMBINADO
    options.GlobalLimiter = PartitionedRateLimiter.CreateChained(
        PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        {
            var userAgent = httpContext.Request.Headers.UserAgent.ToString();
            return RateLimitPartition.GetFixedWindowLimiter(userAgent, partition =>
                new FixedWindowRateLimiterOptions
                {
                    PermitLimit = generalRateLimitOptions.PartitionedPermitLimit,
                    Window = TimeSpan.FromMinutes(generalRateLimitOptions.Window),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                }
            );
        }),
        PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        {
            var userAgent = httpContext.Request.Headers.UserAgent.ToString();
            return RateLimitPartition.GetFixedWindowLimiter(userAgent, partition => 
                new FixedWindowRateLimiterOptions
                {
                    PermitLimit = generalRateLimitOptions.GlobalPermitLimit,
                    Window = TimeSpan.FromHours(1),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                }
            );
        })
    );
});


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.Run();