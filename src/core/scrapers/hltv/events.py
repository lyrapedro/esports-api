from enum import Enum
from bs4 import BeautifulSoup
import json
import re
from playwright.async_api import async_playwright
from contextlib import asynccontextmanager

class EventType(Enum):
    ALL = 0
    MAJOR = 1
    INTERNATIONAL_LAN = 2
    REGIONAL_LAN = 3
    ONLINE = 4
    LOCAL_LAN = 5

@asynccontextmanager 
async def browser_context():
    """Manage browser lifecycle automatically"""
    playwright = await async_playwright().start()
    browser = await playwright.chromium.launch(
        headless=True,
        args=["--no-sandbox", "--disable-dev-shm-usage"]
    )
    try:
        yield browser
    finally:
        await browser.close()
        await playwright.stop()

async def get_current_events(event_type=0):
    """Get current events from HLTV"""
    async with browser_context() as browser:
        page = await browser.new_page(
            user_agent="Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
            extra_http_headers={
                "Accept-Language": "en-US,en;q=0.9",
                "Referer": "https://www.hltv.org/"
            }
        )
        try:
            print(event_type)
            if event_type == EventType.ALL.value:
                await page.goto('https://www.hltv.org/events#tab-ALL')
            else:
                await page.goto(f'https://www.hltv.org/events?eventType={EventType(event_type).name}#tab-ALL')

            await page.wait_for_selector('.tab-content#ALL', timeout=15000)
            content = await page.content()
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

            return result

        except Exception as e:
            return {"error": str(e)}
        finally:
            await page.close()