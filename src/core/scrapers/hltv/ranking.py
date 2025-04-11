# import json

# from bs4 import BeautifulSoup
# import cloudscraper


# class GetRanking:
#     def __init__(self):
#         self.scraper = cloudscraper.create_scraper()

#     def get_ranking(self):
#         html = self.scraper.get('https://www.hltv.org/ranking/teams')

#         if html.status_code != 200:
#             print("Falha ao carregar a página:", html.status_code)

#         bs = BeautifulSoup(html.content, 'html.parser')
#         teams_div = bs.find_all('div', class_='ranked-team')

#         result = []

#         for index, team in enumerate(teams_div):
#             team_name = team.find('span', class_='name').text
#             result.append({"team_position": index + 1, "team_name": team_name})

#         json_result = json.dumps(result, ensure_ascii=False, indent=4)
#         return json_result


# rankingScraper = GetRanking()
# print(rankingScraper.get_ranking())