import json
from src.matches import get_live_matches, get_match_details
from src.events import get_current_events

events_json = get_current_events()

events = json.loads(events_json)

if events:
    event_id = events[0]["event_id"]
    matches_json = get_live_matches(event_id)
    matches = json.loads(matches_json)
    if matches:
        match_url = matches[0]["match_url"]
        match_details = get_match_details(match_url)
        print(match_details)
    else:
        print(json.dumps({"error": "No matches for now"}, indent=4))
else:
    print(json.dumps({"error": "No events for now"}, indent=4))
    
#print("Available events:", events_json)
# matches = get_live_matches(8044)
# print("Live matches:", matches)
# match_details = get_match_details('/matches/2381328/g2-vs-virtuspro-pgl-bucharest-2025')
# print('match details: ', match_details)