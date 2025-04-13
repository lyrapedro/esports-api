import re
from datetime import datetime, timezone
from bs4 import BeautifulSoup
import time
from selenium import webdriver
from selenium.webdriver.chrome.options import Options
import requests

headers = {
    "User-Agent": "Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:52.0) Gecko/20100101 Firefox/52.0",
}

chrome_options = Options()
chrome_options.add_argument("--headless")  # Executar sem abrir janela
chrome_options.add_argument("--disable-gpu")  # Melhora performance em alguns sistemas
chrome_options.add_argument("--window-size=1920x1080")  # Resolução padrão

async def get_live_matches(event_url):
    """Get live matches for an event"""

    driver = webdriver.Chrome(options=chrome_options)
    driver.get(f"https://www.vlr.gg/event/matches{event_url}")

    time.sleep(3)
    content = driver.page_source

    driver.quit()

    bs = BeautifulSoup(content, 'html.parser')
    live_matches = bs.select('a.match-item:has(div.mod-live)')

    result = []

    for match in live_matches:
        teams_div = match.find('div', class_='match-item-vs')
        teams_names = teams_div.find_all('div', 'match-item-vs-team-name')
        team1_name = teams_names[0].find('div', class_='text-of').text #
        print(team1_name)
        team2_name = teams_names[0].find('div', class_='text-of').text #
        print(team2_name)
        match_url = match['href']
        match_page = requests.get(f"https://www.vlr.gg/{match_url}").text
        bs_match = BeautifulSoup(match_page, 'html.parser')
        match_type = bs_match.select_one('.match-header-vs-note') #
        print(match_type)
        map_score = bs_match.select_one('.match-header-vs-score').find_all('span')
        team1_map_score = map_score[0].text #
        print(team1_map_score)
        team2_map_score = map_score[1].text #
        maps_containers = bs_match.find_all('div', class_='vm-stats-game ', attrs={'data-game-id': True})

        maps = []

        for map_container in maps_containers:
            map_name = map_container.find('div', class_='map').select_one('span') #
            team1_score = map_container.find_all('div', class_='team')[0].select_one('div').text #
            team2_score = map_container.find_all('div', class_='team')[1].select_one('div').text #

            maps.append({
                map_name: {
                    team1_name: team1_score,
                    team2_name: team2_score
                }
            })
        
        result.append({
            "match": f"{team1_name} vs {team2_name}",
            "match_type": match_type,
            "map_score": {
                team1_name: team1_map_score,
                team2_name: team2_map_score
            },
            "maps": {
                maps
            }
        })

    return result

def get_match_details(match_url):
    return

def get_all_live_matches():
    return