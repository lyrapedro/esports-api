# Make functions importable from package root
from .events import get_current_events
from .matches import get_live_matches, get_match_details

__all__ = ['get_current_events', 'get_live_matches', 'get_match_details']