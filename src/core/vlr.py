from src.core.scrapers.vlrgg import (
    get_current_events,
    get_live_matches,
    get_all_live_matches
)

class Vlr:
    @staticmethod
    def vlr_live_matches(event_id):
        return get_live_matches(event_id)
    
    @staticmethod
    def vlr_current_events():
        return get_current_events()
    
    @staticmethod
    def vlr_all_live_matches():
        return get_all_live_matches()
    
if __name__ == "__main__":
    print(Vlr.vlr_current_events())