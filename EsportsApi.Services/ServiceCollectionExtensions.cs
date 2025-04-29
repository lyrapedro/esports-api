using EsportsApi.Core.Contracts;
using EsportsApi.Core.Interfaces;
using EsportsApi.Core.Scrapers;
using Microsoft.Extensions.DependencyInjection;
using PuppeteerSharp;

namespace EsportsApi.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddScraperServices(this IServiceCollection services)
    {
        services.AddTransient<ICsScraper, HltvScraper>();
        services.AddScoped<ICsService, CsService>();
        services.AddTransient<IValorantScraper, VlrGgScraper>();
        services.AddScoped<IValorantService, ValorantService>();
        
        return services;
    }
}