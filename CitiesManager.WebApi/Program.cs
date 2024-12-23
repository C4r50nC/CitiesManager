using CitiesManager.WebApi.DatabaseContext;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

// Swagger services
builder.Services.AddEndpointsApiExplorer(); // Generates description for all endpoints
builder.Services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "api.xml"));
}); // Generates Open API specification

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHsts();
app.UseHttpsRedirection();

app.UseSwagger(); // Creates endpoints for swagger.json
app.UseSwaggerUI(); // Creates Swagger UI for testing API endpoints

app.UseAuthorization();

app.MapControllers();

app.Run();
