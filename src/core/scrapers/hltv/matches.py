from bs4 import BeautifulSoup
import json
import time
from playwright.async_api import async_playwright
from contextlib import asynccontextmanager

@asynccontextmanager
async def browser_context():
    """Helper to manage browser lifecycle"""
    playwright = await async_playwright().start()
    browser = await playwright.chromium.launch(
        headless=True,
        args=["--no-sandbox", "--disable-dev-shm-usage"]
    )
    try:
        yield browser
    finally:
        browser.close()

async def get_live_matches(event_id):
    """Get live matches for an event"""
    async with browser_context() as browser:
        page = await browser.new_page(
            user_agent="Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
            extra_http_headers={
                "Accept-Language": "en-US,en;q=0.9",
                "Referer": "https://www.hltv.org/",
                "Accept": "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8"
            }
        )
        try:
            await page.goto(f"https://www.hltv.org/events/{event_id}/matches", timeout=60000)
            await page.wait_for_selector(".liveMatches", timeout=15000)
            time.sleep(2)
            content = await page.content()
            
            bs = BeautifulSoup(content, "html.parser")
            live_matches = bs.find_all('div', class_='match', attrs={'data-livescore-match': True})
            
            result = []
            for match in live_matches:
                match_id = match['data-livescore-match']
                teams = match.find_all("div", class_="match-team")
                team1_name = teams[0].find("div", class_="match-teamname").text
                team2_name = teams[1].find("div", class_="match-teamname").text
                match_url = match.find('a')['href']
                
                result.append({
                    "match_id": match_id,
                    "team1_name": team1_name,
                    "team2_name": team2_name,
                    "match_url": match_url
                })
            
            return result
        
        except Exception as e:
            return {"error": str(e)}
        finally:
            page.close()

async def get_match_details(match_url):
    """Get details for a specific match"""
    async with browser_context() as browser:
        page = await browser.new_page(
            user_agent="Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
            extra_http_headers={
                "Accept-Language": "en-US,en;q=0.9",
                "Referer": "https://www.hltv.org/",
            }
        )
        try:
            await page.goto(f"https://www.hltv.org{match_url}", timeout=60000)
            await page.wait_for_selector(".mapholder", timeout=15000)
            time.sleep(2)
            content = await page.content()
            
            bs = BeautifulSoup(content, "html.parser")
            maps_div = bs.find_all("div", class_="mapholder")
            
            match_maps = []
            for map_div in maps_div:
                map_name = map_div.find('img')['alt']
                scores = map_div.find_all('div', class_='results-team-score')
                teams = map_div.find_all("div", class_="results-teamname text-ellipsis")
                match_maps.append({
                    "map_name": map_name,
                    teams[0].text: scores[0].text.strip(),
                    teams[1].text: scores[1].text.strip()
                })
            
            return {"match_maps": match_maps}
        
        except Exception as e:
            return {"error": str(e)}
        finally:
            await page.close()