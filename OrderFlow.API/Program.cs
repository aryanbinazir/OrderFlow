using Microsoft.OpenApi;
using TicketingService.API.DI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.RegisterSwagger();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options => options.OpenApiVersion = OpenApiSpecVersion.OpenApi2_0);
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderFlow v1"); });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
