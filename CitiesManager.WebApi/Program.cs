using Asp.Versioning;
using CitiesManager.Core.Identities;
using CitiesManager.Core.ServiceContracts;
using CitiesManager.Core.Services;
using CitiesManager.Infrastructure.DatabaseContexts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ProducesAttribute("application/json"));
    options.Filters.Add(new ConsumesAttribute("application/json"));

    AuthorizationPolicy authorizationPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    options.Filters.Add(new AuthorizeFilter(authorizationPolicy)); // Apply authorization policy for all controllers globally
}).AddXmlSerializerFormatters(); // Must be added to produce XML data

builder.Services
    .AddApiVersioning(options =>
    {
        // Read version number from request URL
        options.ApiVersionReader = new UrlSegmentApiVersionReader(); // Using URL parameter "apiVersion"
        // options.ApiVersionReader = new QueryStringApiVersionReader(); // Using query string, "api-version" by default
        // options.ApiVersionReader = new HeaderApiVersionReader("api-version"); // Using request header parameter defined by user

        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV"; // Version number formatted as 'v'major[.minor][-status]
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

// Swagger services
builder.Services.AddEndpointsApiExplorer(); // Generates description for all endpoints
builder.Services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "api.xml"));
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
    {
        Title = "Cities Web API",
        Version = "v1",
    });
    options.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo()
    {
        Title = "Cities Web API",
        Version = "v2",
    });
}); // Generates Open API specification

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        string[]? origins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
        if (origins != null)
        {
            policy.WithOrigins(origins);
        }

        policy
            .WithHeaders("Authorization", "origin", "accept", "content-type")
            .WithMethods("GET", "POST", "PUT", "DELETE");
    });

    options.AddPolicy("CustomRestrictedPolicy", policy =>
    {
        string[]? origins = builder.Configuration.GetSection("AllowedRestrictedOrigins").Get<string[]>();
        if (origins != null)
        {
            policy.WithOrigins(origins);
        }

        policy
            .WithHeaders("Authorization", "origin", "accept")
            .WithMethods("GET");
    });
});

builder.Services
    .AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.Password.RequiredLength = 4;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireDigit = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
    .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();

builder.Services.AddTransient<IJwtService, JwtService>();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Backup authentication method when login fail
    })
    .AddJwtBearer(options =>
    {
        string? secretKey = builder.Configuration["Jwt:Key"];
        if (secretKey == null)
        {
            throw new NullReferenceException("Secret key is null");
        }

        options.TokenValidationParameters = new()
        {
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey)),
        };
    });

builder.Services.AddAuthentication();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSwagger(); // Creates endpoints for swagger.json
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("v1/swagger.json", "1.0");
    options.SwaggerEndpoint("v2/swagger.json", "2.0");
}); // Creates Swagger UI for testing API endpoints

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
