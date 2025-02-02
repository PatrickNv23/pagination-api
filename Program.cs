using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PaginationResultWebApi.Data;
using PaginationResultWebApi.Data.Configuration;
using PaginationResultWebApi.Repositories;
using PaginationResultWebApi.Repositories.Contracts;
using PaginationResultWebApi.Services;
using PaginationResultWebApi.Services.Contracts;

var builder = WebApplication.CreateBuilder(args);

// CORS NAME
var corsPolicyName = "AllowSpecificOrigins";

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddScoped<IGuitarService, GuitarService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IGuitarRepository, GuitarRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));


// MediatR
builder.Services.AddMediatR(
    cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly)
    );

// RATE LIMITING
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

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithOrigins("http://localhost:4200", "https://localhost:4200");
    });
});

// GOOGLE AUTH AND JWT AUTH
// builder.Services.AddAuthentication(options => 
//     {
//         options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//         options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
//     })
//     .AddCookie()
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    })
    .AddGoogle(GoogleDefaults.AuthenticationScheme, googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Google:ClientId"]!;
        googleOptions.ClientSecret = builder.Configuration["Google:ClientSecret"]!;
        
        googleOptions.Events.OnRedirectToAuthorizationEndpoint = context =>
        {
            context.Response.Redirect(context.RedirectUri + "&prompt=select_account");
            return Task.CompletedTask;
        };

        googleOptions.Events.OnCreatingTicket = context =>
        {
            var picture = context.User.GetProperty("picture").GetString();

            if (!string.IsNullOrEmpty(picture))
            {
                context?.Identity?.AddClaim(new Claim("picture", picture));
            }

            return Task.CompletedTask;
        };
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

app.UseCors(corsPolicyName);

app.UseRateLimiter();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();