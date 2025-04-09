import json
from src.matches import get_live_matches, get_match_details
from src.events import get_current_events

# events_json = get_current_events()
# print("Available events:", events_json)

# events = json.loads(events_json)

# if events:
matches = get_live_matches(8408)
print("Live matches:", matches)