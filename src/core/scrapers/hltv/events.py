from enum import Enum
from bs4 import BeautifulSoup
import time
import re
import cloudscraper

class EventType(Enum):
    ALL = 0
    MAJOR = 1
    INTERNATIONAL_LAN = 2
    REGIONAL_LAN = 3
    ONLINE = 4
    LOCAL_LAN = 5

async def get_current_events(event_type=0):
    """Get current events from HLTV"""
    scraper = cloudscraper.create_scraper()
    try:
        content = ""
        if event_type == EventType.ALL.value:
            content = scraper.get('https://www.hltv.org/events#tab-ALL').text
        else:
            content = scraper.get(f'https://www.hltv.org/events?eventType={EventType(event_type).name}#tab-ALL').text

        for attempt in range(5):
            bs = BeautifulSoup(content, 'html.parser')
            target_element = bs.select_one('.tab-content#ALL')
            if target_element:
                print("Found the element!")
                break
            else:
                print(f"Attempt {attempt + 1}: Element not found, waiting...")
                time.sleep(5)
        
        current_events_section = bs.select_one('.tab-content#ALL')
        events = current_events_section.find_all('a')
        pattern = r'\d+'
        
        result = []
        for event in events:
            event_name = event.find('td', class_='event-name-col').find('div', class_='text-ellipsis').text
            event_id = re.findall(pattern, event['href'])[0]
            result.append({
                "event_id": event_id,
                "event_name": event_name,
                "type": EventType(event_type).name
            })

        return result

    except Exception as e:
        return {"error": str(e)}