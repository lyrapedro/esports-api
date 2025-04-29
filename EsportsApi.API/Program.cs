using EsportsApi.Core.Interfaces;
using EsportsApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScraperServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/vlr/events", (IValorantService valorantService) =>
    {
        var events = valorantService.GetLiveEvents();

        return events;
    })
.WithName("VlrGetLiveEvents")
.WithOpenApi();

app.MapGet("/vlr/event/{id}/matches", (int id, IValorantService valorantService) =>
    {
        var matches = valorantService.GetLiveMatches(id);

        return matches;
    })
    .WithName("VlrGetLiveMatchesFromEvent")
    .WithOpenApi();

app.MapGet("/cs/events", (ICsService csService) =>
    {
        var matches = csService.GetLiveEvents();

        return matches;
    })
    .WithName("CsGetLiveEvents")
    .WithOpenApi();

app.Run();
