from selenium_stealth import stealth
from bs4 import BeautifulSoup
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
import json


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

        WebDriverWait(self.driver, 45).until(EC.visibility_of_element_located((By.CLASS_NAME, 'currentMapScore')))

        html = self.driver.page_source

        bs = BeautifulSoup(html, "html.parser")

        live_matches = bs.find_all("div", class_="liveMatch-container")
        result = []

        for match in live_matches:
            #TODO: GET CURRENT MAP NAME
            # expand_score_buttons = driver.find_elements(By.CLASS_NAME, 'expand-match-btn')
            # for button in expand_score_buttons:
            #     button.click()
            # current_map = match.find("div", class_="scorebot-container").find("div", class_="live-text")
            # print(current_map)
            match_id = match.find("div", class_="liveMatch")['data-livescore-match']
            teams = match.find_all("div", class_="matchTeam")
            team1_name = teams[0].find("div", class_="matchTeamName").text
            team1_score_section = teams[0].find("div", class_="matchTeamScore")
            team1_current_score = team1_score_section.find("span", class_="currentMapScore").text.strip()
            team1_current_map_score = team1_score_section.find("span", class_="mapScore").find("span").text.strip()
            team2_name = teams[1].find("div", class_="matchTeamName").text
            team2_score_section = teams[1].find("div", class_="matchTeamScore")
            team2_current_score = team2_score_section.find("span", class_="currentMapScore").text.strip()
            team2_current_map_score = team2_score_section.find("span", class_="mapScore").find("span").text.strip()
            result.append({"match_id": match_id, "team1": {"name": team1_name, "currentScore": team1_current_score, "mapsWon": team1_current_map_score},
                           "team2": {"name": team2_name, "currentScore": team2_current_score, "mapsWon": team2_current_map_score}})

        self.driver.quit()
        json_result = json.dumps(result, ensure_ascii=False, indent=4)
        return json_result


matchesScraper = GetMatches()
print(matchesScraper.get_matches(7720))


