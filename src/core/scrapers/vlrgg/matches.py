import re
from datetime import datetime, timezone
from bs4 import BeautifulSoup
import time
import requests

headers = {
    "User-Agent": "Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:52.0) Gecko/20100101 Firefox/52.0",
}

async def get_live_matches(event_url):
    """Get live matches for an event"""

    url = f'https://www.vlr.gg/event/matches{event_url}'

    content = requests.get(url).text

    bs = BeautifulSoup(content, 'html.parser')

    live_matches = []
    for a_tag in bs.find_all('a', class_='wf-module-item'):
        ml_status = a_tag.find('div', class_='ml-status')
        if ml_status and ml_status.get_text(strip=True) == 'LIVE':
            live_matches.append(a_tag)

    result = []

    for match in live_matches:
        teams_div = match.find('div', class_='match-item-vs')
        teams_names = teams_div.find_all('div', 'match-item-vs-team-name')
        team1_name = teams_names[0].find('div', class_='text-of').text.strip()
        team2_name = teams_names[1].find('div', class_='text-of').text.strip()
        match_url = match['href']
        match_page = requests.get(f"https://www.vlr.gg{match_url}").text
        bs_match = BeautifulSoup(match_page, 'html.parser')
        match_type = bs_match.select_one('.match-header-vs-note').text.strip()
        map_score = bs_match.select_one('.match-header-vs-score').find_all('span')
        team1_map_score = map_score[0].text.strip()
        team2_map_score = map_score[2].text.strip() # 2 because index 1 is the separator (:)
        maps_containers = bs_match.find_all('div', class_='vm-stats-game', attrs={'data-game-id': True})
        map_divs = [div for div in maps_containers if div['data-game-id'] != 'all']

        maps = []

        for map_div in map_divs:
            map_name = map_div.find('div', class_='map').select_one('span').find(text=True, recursive=False).strip() #only the inner text
            team1_score = map_div.find_all('div', class_='team')[0].select_one('.score').text.strip()
            team2_score = map_div.find_all('div', class_='team')[1].select_one('.score').text.strip()

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
            "maps": maps
        })

    return result

def get_match_details(match_url):
    return

def get_all_live_matches():
    return