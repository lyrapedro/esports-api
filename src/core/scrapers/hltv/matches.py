from bs4 import BeautifulSoup
import time
import cloudscraper

async def get_live_matches(event_id):
    """Get live matches for an event"""

    scraper = cloudscraper.create_scraper()

    try:
        content = scraper.get(f"https://www.hltv.org/events/{event_id}/matches").text

        for attempt in range(5):
            bs = BeautifulSoup(content, 'html.parser')
            target_element = bs.select_one('.liveMatches')
            if target_element:
                print("Found the element!")
                break
            else:
                print(f"Attempt {attempt + 1}: Element not found, waiting...")
                time.sleep(5)
        
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
                time.sleep(5)
        
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