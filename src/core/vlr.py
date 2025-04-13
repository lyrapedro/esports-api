from src.core.scrapers.vlrgg import (
    get_current_events,
    get_live_matches,
    get_match_details
)

class Vlr:
    @staticmethod
    def vlr_live_matches(event_id):
        return get_live_matches(event_id)
    
    @staticmethod
    def vlr_current_events():
        return get_current_events()
    
    @staticmethod
    def vlr_match_details(match_url):
        return get_match_details(match_url)
    
if __name__ == "__main__":
    print(Vlr.vlr_current_events())