import re
from datetime import datetime, timezone
from bs4 import BeautifulSoup
import json
import requests

async def get_current_events():
    """Get current events from VLR.GG"""

    content = requests.get("https://www.vlr.gg").text
    bs = BeautifulSoup(content, 'html.parser')

    live_events_h1 = bs.find('h1', class_='wf-label mod-sidebar', string=lambda text: 'live events' in text.lower().strip())

    target_div = live_events_h1.find_next_sibling('div', class_='wf-module wf-card mod-sidebar')

    live_events = target_div.select('a.wf-module-item.event-item')

    result = []

    for event in live_events:
        event_name = event.find('div', class_='event-item-name').text.strip()
        event_url = event['href']
        match = re.search(r'/event/(\d+)', event_url)
        event_id = match.group(1)
        event_region = event.find('div', class_="event-item-tag").text.strip()

        result.append({
            "id": event_id,
            "event_name": event_name,
            "event_url": event_url,
            "region": event_region
        })

    return result
