from bs4 import BeautifulSoup
import time
import cloudscraper

async def get_live_matches(event_id):
    """Get live matches for an event"""

    scraper = cloudscraper.create_scraper()

    try:
        content = scraper.get(f"https://www.hltv.org/events/{event_id}/matches").text

        for attempt in range(5):
            if attempt + 1 == 5:
                return {"no matches at the moment"}
            bs = BeautifulSoup(content, 'html.parser')
            target_element = bs.select_one('.liveMatches')
            if target_element:
                print("Found the element!")
                break
            else:
                print(f"Attempt {attempt + 1}: Element not found, waiting...")
                time.sleep(3)
        
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

async def get_match_details(match_url):
    """Get details for a specific match"""

    scraper = cloudscraper.create_scraper()

    try:
        content = scraper.get(f"https://www.hltv.org{match_url}").text

        for attempt in range(5):
            bs = BeautifulSoup(content, 'html.parser')
            target_element = bs.select_one('.mapholder')
            if target_element:
                print("Found the element!")
                break
            else:
                print(f"Attempt {attempt + 1}: Element not found, waiting...")
                time.sleep(3)
        
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
    
async def get_all_live_matches():
    """Get score for all live matches"""

    scraper = cloudscraper.create_scraper()

    try:
        content = scraper.get("https://www.hltv.org/matches").text

        for attempt in range(5):
            if attempt + 1 == 5:
                return {"no matches at the moment"}
            bs = BeautifulSoup(content, "html.parser")
            target_element = bs.select_one('.liveMatches')
            if (target_element):
                break
            else:
                print(f"Attempt {attempt + 1}: Element not found, waiting...")
                time.sleep(3)
        
        result = []
        live_matches = bs.find_all('div', class_='match', attrs={'data-livescore-match': True})
        for match in live_matches:
            teams_details = match.find_all("div", class_="match-team")
            team1_name = teams_details[0].find("div", class_="match-teamname").text
            team2_name = teams_details[1].find("div", class_="match-teamname").text
            team1_current_score = teams_details[2].find('span', class_='current-map-score').text
            team2_current_score = teams_details[3].find('span', class_='current-map-score').text
            team1_map_score = teams_details[2].find('span', attrs={'data-livescore-maps-won-for': True}).text
            team2_map_score = teams_details[3].find('span', attrs={'data-livescore-maps-won-for': True}).text
            match_type = match.select_one('.match-meta').text
            match_event_name = match.select_one('.match-event .text-ellipsis')['data-event-headline']
            result.append({
                "event_name": match_event_name,
                "match_type": match_type,
                "map_score": {
                    team1_name: team1_map_score,
                    team2_name: team2_map_score
                },
                "current_map_score": {
                    team1_name: team1_current_score,
                    team2_name: team2_current_score
                }
            })
    except Exception as e:
        return {"error": str(e)}