using EsportsApi.Core.Interfaces;
using EsportsApi.Core.Scrapers;
using Microsoft.Extensions.DependencyInjection;

namespace EsportsApi.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddScraperServices(this IServiceCollection services)
    {
        services.AddScoped<VlrGgScraper>();
        services.AddScoped<IValorantService, ValorantService>();
        
        return services;
    }
}