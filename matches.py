import time

from selenium_stealth import stealth
from bs4 import BeautifulSoup
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
import json
import re


class GetMatches:
    def __init__(self):
        self.options = webdriver.ChromeOptions()
        self.options.add_argument("start-maximized")
        self.options.add_argument("--headless")
        self.options.add_experimental_option("excludeSwitches", ["enable-automation"])
        self.options.add_experimental_option('useAutomationExtension', False)
        self.driver = webdriver.Chrome(options=self.options)

    def get_matches(self, event_id):
        stealth(self.driver,
                languages=["en-US", "en"],
                vendor="Google Inc.",
                platform="Win32",
                webgl_vendor="Intel Inc.",
                renderer="Intel Iris OpenGL Engine",
                fix_hairline=True,
                )

        self.driver.get("https://www.hltv.org/events/{}/matches".format(event_id))

        html = self.driver.page_source

        bs = BeautifulSoup(html, "html.parser")

        live_matches = bs.find_all("div", class_="liveMatch")
        result = []

        for match in live_matches:
            match_id = match['data-livescore-match']
            teams = match.find_all("div", class_="matchTeam")
            team1_name = teams[0].find("div", class_="matchTeamName").text
            team2_name = teams[1].find("div", class_="matchTeamName").text
            match_url = match.find('a', class_='match')['href']
            self.driver.get("https://www.hltv.org{}".format(match_url))
            self.driver.implicitly_wait(10)
            match_html = self.driver.page_source
            bs = BeautifulSoup(match_html, "html.parser")
            maps_div = bs.find_all("div", class_="mapholder")

            match_maps = []

            for map_div in maps_div:
                map_name = map_div.find('img')['alt']
                scores_section = map_div.find_all('div', class_='results-team-score')
                team1_current_score = scores_section[0].text.strip()
                team2_current_score = scores_section[1].text.strip()
                match_maps.append({"map_name": map_name, "team1_score": team1_current_score, "team2_score": team2_current_score})

            result.append({"match_id": match_id, "team1_name": team1_name, "team2_name": team2_name, "match_maps": match_maps})

        self.driver.quit()
        json_result = json.dumps(result, ensure_ascii=False, indent=4)
        return json_result


matchesScraper = GetMatches()
print(matchesScraper.get_matches(7666))


