from core.scrapers.hltv import (
    get_match_details,
    get_current_events,
    get_live_matches
)

class Cs:
    @staticmethod
    async def cs_live_matches(event_id):
        return await get_live_matches(event_id)
    
    @staticmethod
    async def cs_current_events(event_type):
        return await get_current_events(event_type)
    
    @staticmethod
    async def cs_match_details(match_url):
        return await get_match_details(match_url)
    
if __name__ == "__main__":
    print(Cs.cs_current_events())