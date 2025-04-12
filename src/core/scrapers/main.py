import requests
import cloudscraper

def check_health():
    sites = ["https://esportsapi.vercel.app", "https://vlr.gg", "https://hltv.org"]
    results = {}
    for site in sites:
        try:
            scraper = cloudscraper.create_scraper()
            response = scraper.get(site)
            results[site] = {
                "status": "Healthy" if response.status_code == 200 else "Unhealthy",
                "status_code": response.status_code,
            }
        except requests.RequestException:
            results[site] = {"status": "Unhealthy", "status_code": None}
    return results