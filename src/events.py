from enum import Enum
from bs4 import BeautifulSoup
import json
import re
from playwright.sync_api import sync_playwright
from contextlib import contextmanager

class EventType(Enum):
    ALL = 0
    MAJOR = 1
    INTERNATIONAL_LAN = 2
    REGIONAL_LAN = 3
    ONLINE = 4
    LOCAL_LAN = 5

@contextmanager
def browser_context():
    """Manage browser lifecycle automatically"""
    with sync_playwright() as playwright:
        browser = playwright.chromium.launch(
            headless=True,
            args=["--no-sandbox", "--disable-dev-shm-usage"]
        )
        try:
            yield browser
        finally:
            browser.close()

def get_current_events(event_type=0):
    """Get current events from HLTV"""
    with browser_context() as browser:
        page = browser.new_page(
            user_agent="Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
            extra_http_headers={
                "Accept-Language": "en-US,en;q=0.9",
                "Referer": "https://www.hltv.org/"
            }
        )
        try:
            if event_type == EventType.ALL.value:
                page.goto('https://www.hltv.org/events#tab-ALL')
            else:
                page.goto(f'https://www.hltv.org/events?eventType={EventType(event_type).name}#tab-ALL')

            page.wait_for_selector('.tab-content#ALL', timeout=15000)
            content = page.content()
            bs = BeautifulSoup(content, 'html.parser')
            
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

            return json.dumps(result, ensure_ascii=False, indent=4)

        except Exception as e:
            return json.dumps({"error": str(e)}, indent=4)
        finally:
            page.close()