from fastapi import APIRouter, Query, Request
from slowapi import Limiter
from slowapi.util import get_remote_address

from src.core.scrapers.main import check_health
from src.core.cs import Cs
from src.core.vlr import Vlr

router = APIRouter()
limiter = Limiter(key_func=get_remote_address)
cs = Cs()
vlr = Vlr()

@router.get("/cs/events")
@limiter.limit("100/minute")
async def CS_events(request: Request, event_type: int = 0):
    """
    query parameters:\n
        "event_type": type of event\n
            ALL = 0
            MAJOR = 1
            INTERNATIONAL_LAN = 2
            REGIONAL_LAN = 3
            ONLINE = 4
            LOCAL_LAN = 5
    """
    return await cs.cs_current_events(event_type)

@router.get("/cs/matches")
@limiter.limit("100/minute")
async def CS_matches(request: Request, event_id: str):
    """
        query parameters:\n
            "event_id": ID of event\n
    """
    return await cs.cs_live_matches(event_id)

@router.get("/cs/match")
@limiter.limit("100/minute")
async def CS_matches(request: Request, match_url: str):
    """
        query parameters:\n
            "match_url": Partial URL of match (returned from /cs/matches response)\n
    """
    return await cs.cs_match_details(match_url)

@router.get("/health")
def health():
    return check_health()