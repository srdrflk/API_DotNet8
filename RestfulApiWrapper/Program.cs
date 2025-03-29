using Microsoft.AspNetCore.Mvc;
using RestfulApiWrapper.Middleware;
using RestfulApiWrapper.Models;
using RestfulApiWrapper.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});

// Configure API behavior
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
    options.InvalidModelStateResponseFactory = context =>
    {
        var errorResponse = ApiErrorResponse.FromModelState(
            context.ModelState,
            context.HttpContext);
        return new BadRequestObjectResult(errorResponse);
    };
});

// Add HttpClient and services
builder.Services.AddHttpClient<IRestfulApiService, RestfulApiService>();
builder.Services.AddScoped<IRestfulApiService, RestfulApiService>();


// Add Swagger for testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add the middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
