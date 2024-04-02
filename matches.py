from bs4 import BeautifulSoup
import re
import cloudscraper
import json


def get_matches(event_id):
    scraper = cloudscraper.create_scraper()

    html = scraper.get("https://www.hltv.org/events/{}/matches".format(event_id))
    bs = BeautifulSoup(html.content, "html.parser")

    current_matches = bs.find_all("div")  # TODO: TAKE CURRENT MATCHES
