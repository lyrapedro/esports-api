using EsportsApi.Core.Contracts;
using EsportsApi.Core.Interfaces;
using EsportsApi.Core.Scrapers.VlrGG;
using Microsoft.Extensions.DependencyInjection;

namespace EsportsApi.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddScraperServices(this IServiceCollection services)
    {
        services.AddScoped<VlrGGScraper>();
        services.AddScoped<IValorantService, ValorantService>();
        
        return services;
    }
}