using EsportsApi.Application.Contracts;
using EsportsApi.Application.Scrapers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICsScraper, HltvScraper>();
builder.Services.AddScoped<IValorantScraper, VlrGgScraper>();
builder.Services.AddHttpClient("VlrGGClient", config =>
{
    config.BaseAddress = new Uri("https://vlr.gg/");
    config.Timeout = new TimeSpan(0, 0, 15);
    config.DefaultRequestHeaders.Clear();
});
builder.Services.AddHttpClient("Browserless", config =>
{
    config.BaseAddress = new Uri($"{builder.Configuration["BrowserlessIO:Endpoint"]}?token={builder.Configuration["BrowserlessIO:Token"]}");
    config.Timeout = new TimeSpan(0, 0, 15);
    config.DefaultRequestHeaders.Clear();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/vlr/events", (IValorantScraper valorantScraper) =>
    {
        var events = valorantScraper.GetLiveEvents();

        return events;
    })
.WithName("VlrGetLiveEvents")
.WithOpenApi();

app.MapGet("/vlr/event/{id}/matches", (int id, IValorantScraper valorantScraper) =>
    {
        var matches = valorantScraper.GetLiveMatches(id);

        return matches;
    })
    .WithName("VlrGetLiveMatchesFromEvent")
    .WithOpenApi();

app.MapGet("/cs/events", (ICsScraper csScraper) =>
    {
        var matches = csScraper.GetLiveEvents();

        return matches;
    })
    .WithName("CsGetLiveEvents")
    .WithOpenApi();

app.Run();
