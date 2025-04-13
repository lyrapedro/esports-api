import re
from datetime import datetime, timezone
from bs4 import BeautifulSoup
import json
import requests

async def get_current_events():
    """Get current events from VLR.GG"""

    content = requests.get("https://www.vlr.gg/events").text
    bs = BeautifulSoup(content, 'html.parser')
    ongoing_events = bs.select('a.event-item:has(span.mod-ongoing)')

    result = []

    for event in ongoing_events:
        event_name = event.find('div', class_='event-item-title').text.strip()
        print(event_name)
        event_url = event['href']
        print(event_url)
        match = re.search(r'/event/(\d+)', event_url)
        event_id = match.group(1)
        print(event_id)
        region_flag = event.find('i', class_='flag')
        event_region = str(region_flag).split('mod-')[1].split('"')[0]

        result.append({
            "id": event_id,
            "event_name": event_name,
            "event_url": event_url,
            "region": event_region
        })

    return result
