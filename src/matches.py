from bs4 import BeautifulSoup
import json
import time
from playwright.sync_api import sync_playwright


class GetMatches:
    def __init__(self):
        self.playwright = sync_playwright().start()
        self.browser = self.playwright.chromium.launch(
            headless=True,
            args=["--no-sandbox", "--disable-dev-shm-usage"]
        )

    def get_matches(self, event_id):
        page = self.browser.new_page(
            user_agent="Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
            extra_http_headers={
                "Accept-Language": "en-US,en;q=0.9",
                "Referer": "https://www.hltv.org/"
            }
        )
        result = []
        try :
            page.goto(f"https://www.hltv.org/events/{event_id}/matches", timeout=60000)

            page.wait_for_selector(".liveMatches", timeout=15000)

            time.sleep(2)

            content = page.content()

            bs = BeautifulSoup(content, "html.parser")

            live_matches = bs.find_all("div", class_="match")

            for match in live_matches:
                match_id = match['data-livescore-match']
                teams = match.find_all("div", class_="match-team")
                team1_element = teams[0].find("div", class_="match-teamname")
                team2_element = teams[1].find("div", class_="match-teamname")
                team1_name = team1_element.text
                team2_name = team2_element.text
                print(f"{team1_name} vs {team2_name}")
                #erro a partir daq
                match_url = match.find('a', class_='match')['href']
                print(match_url)
                page.goto(f"https://www.hltv.org{match_url}", timeout=60000)
                page.wait_for_selector(".mapholder", timeout=15000)
                time.sleep(2)
                match_content = page.content()
                bs = BeautifulSoup(match_content, "html.parser")
                maps_div = bs.find_all("div", class_="mapholder")

                match_maps = []

                for map_div in maps_div:
                    map_name = map_div.find('img')['alt']
                    scores_section = map_div.find_all('div', class_='results-team-score')
                    team1_current_score = scores_section[0].text.strip()
                    team2_current_score = scores_section[1].text.strip()
                    match_maps.append({"map_name": map_name, "team1_score": team1_current_score, "team2_score": team2_current_score})

                result.append({"match_id": match_id, "team1_name": team1_name, "team2_name": team2_name, "match_maps": match_maps})
        
        except Exception as e:
            return json.dumps({"error": str(e)}, indent=4)
        finally:
            page.close()
        
        return json.dumps(result, ensure_ascii=False, indent=4)
    
    def __del__(self):
        self.browser.close()
        self.playwright.stop()


matchesScraper = GetMatches()
print(matchesScraper.get_matches(8294))


