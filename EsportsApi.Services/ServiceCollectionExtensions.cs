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
        services.AddSingleton<IBrowser>(provider =>
        {
            // Run the async initialization synchronously
            return Task.Run(async () =>
            {
                // Download the Chromium binary if missing
                var browserFetcher = new BrowserFetcher();
                await browserFetcher.DownloadAsync();

                // Launch the browser with options
                return await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true,
                    Args = new[] { "--no-sandbox" }
                });
            }).GetAwaiter().GetResult(); // Block until the browser is launched
        });
        
        services.AddTransient<IValorantScraper, VlrGgScraper>();
        services.AddScoped<IValorantService, ValorantService>();
        
        return services;
    }
}