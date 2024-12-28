using Asp.Versioning;
using CitiesManager.WebApi.DatabaseContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ProducesAttribute("application/json"));
    options.Filters.Add(new ConsumesAttribute("application/json"));
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

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHsts();
app.UseHttpsRedirection();

app.UseSwagger(); // Creates endpoints for swagger.json
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("v1/swagger.json", "1.0");
    options.SwaggerEndpoint("v2/swagger.json", "2.0");
}); // Creates Swagger UI for testing API endpoints

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
