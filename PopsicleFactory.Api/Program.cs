using FluentValidation;
using PopsicleFactory.Api.Middleware;
using PopsicleFactory.Api.Repositories;
using PopsicleFactory.Api.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<PopsicleModelValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Configure dependency injection
builder.Services.AddScoped<IInventoryRepository, InMemoryInventoryRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<RequestLoggingMiddleware>();

app.MapControllers();

app.Run();
