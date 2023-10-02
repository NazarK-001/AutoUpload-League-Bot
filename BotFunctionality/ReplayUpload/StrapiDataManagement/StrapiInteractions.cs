using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using jsonFieldsAndElo;
using StrapiFunctionality;

namespace StrapiDataManagement
{
    internal class ParsingPlayerStrapiData
    {
        private string playerData;

        public ParsingPlayerStrapiData(string playerStrapiString)
        {
            playerData = playerStrapiString;
        }

        public int GetPlayersElo(string playerNameToRetrieveElo)
        {
            string pattern = $@"""name"":""{Regex.Escape(playerNameToRetrieveElo)}"".*?""elo"":(\d+)";
            Match match = Regex.Match(playerData, pattern);
            if (match.Success && int.TryParse(match.Groups[1].Value, out int elo))
            {
                return elo;
            }
            return -1;
        }

        public JArray GetPlayersHistoric(string playerNameToRetrieveHistoric)
        {
            string pattern = $@"""name"":""{Regex.Escape(playerNameToRetrieveHistoric)}"".*?""historic"":(null|\[.*?\])";

            Match match = Regex.Match(playerData, pattern);
            if (match.Success)
            {
                string historicArrayJson = match.Groups[1].Value;
                if (historicArrayJson == "null")
                {
                    return new JArray();
                }
                return JArray.Parse(historicArrayJson);
            }
            return new JArray();
        }

        public string GetPlayersID(string playerNameToRetrieveID)
        {
            string pattern = $@"{{""id"":(\d+),""attributes"":{{""name"":""{Regex.Escape(playerNameToRetrieveID)}""";
            Match match = Regex.Match(playerData, pattern);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return null;
        }

    }

    internal class GettingEntireData  // this one needs to be used only once!
    {
        private GetPostPut Tools;
        private Dictionary<string, string> EndpointsDictionary;
        private string PlayersResponse;
        //private string MapsResponse;

        public GettingEntireData()
        {
            StrapiEndpoints e = new StrapiEndpoints();
            EndpointsDictionary = e.GetDictionary();
            Tools = new GetPostPut();
            PlayersResponse = Tools.Get(EndpointsDictionary["Players"]);
            //MapsResponse = Tools.Get(EndpointsDictionary["Maps"]);
        }

        public string GetPlayersResponse()
        {
            return PlayersResponse;
        }

        public List<string> GetListOfNicks()
        {
            return ExtractList(PlayersResponse, "name");
        }

        private List<string> ExtractList(string response, string field)
        {
            List<string> data = new List<string>();
            string pattern = $@"""{field}"":""([^""]+)""";

            foreach (Match match in Regex.Matches(response, pattern))
            {
                string item = match.Groups[1].Value;
                data.Add(item);
            }
            return data;
        }
    }

    internal class PostingStrapiData
    {
        private GetPostPut Tools;
        private Dictionary<string, string> EndpointsDictionary;
        private EloRelatedJSON Fields;
        string IdPlayerWin;
        string IdPlayerLose;

        public PostingStrapiData()
        {
            StrapiEndpoints e = new StrapiEndpoints();
            EndpointsDictionary = e.GetDictionary();
            Tools = new GetPostPut();
        }

        public async Task<string> NewFileUpload (Stream FileStream, string fileName)
        {
            return await Tools.Post(EndpointsDictionary["Upload"], FileStream, fileName);
        }

        public void NewGameEntry(string date_start, string date_end, string id_faction_win, string id_faction_lose, string id_player_win, string id_player_lose, string FileId, string MapId, string WinnerName, string LoserName, int WinnerEloBefore, int LoserEloBefore)
        {
            IdPlayerWin = id_player_win;
            IdPlayerLose = id_player_lose;
            Fields = new EloRelatedJSON (Convert.ToInt32(id_player_win), WinnerName, WinnerEloBefore, Convert.ToInt32(id_player_lose), LoserName, LoserEloBefore);
            JObject eloFieldJson = JObject.Parse(Fields.GetReplayEloField());
            var GameData = new
            {
                data = new
                {
                    date_start = date_start,
                    date_end = date_end,
                    elo = new[]
                    {
                        eloFieldJson,
                    },
                    replay = new
                    {
                        id = FileId,
                    },
                    faction_win = new
                    {
                        id = id_faction_win
                    },
                    faction_lose = new
                    {
                        id = id_faction_lose
                    },
                    player_win = new
                    {
                        id = id_player_win,
                    },
                    player_lose = new
                    {
                        id = id_player_lose,
                    },
                    map = new
                    {
                        id = MapId,
                    }
                }
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(GameData);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            Tools.Post(EndpointsDictionary["Games"], content);
        }

        public void ModifyPlayer(JArray NonupdatedHistoricWinner, JArray NonupdatedHistoricLoser)
        {
            JArray UpdatedHistoricWinner = Fields.UpdateHistoricFieldWinner(NonupdatedHistoricWinner);
            JArray UpdatedHistoricLoser = Fields.UpdateHistoricFieldLoser(NonupdatedHistoricLoser);
            var updatedHistoricW = new
            {
                data = new
                {
                    historic = UpdatedHistoricWinner, 
                    elo = Fields.GetResultRatingWinner()
                }
            };

            var updatedHistoricL = new
            {
                data = new
                {
                    historic = UpdatedHistoricLoser,
                    elo = Fields.GetResultRatingLoser()
                }
            };

            var JsonWin = Newtonsoft.Json.JsonConvert.SerializeObject(updatedHistoricW);
            var ContentW = new StringContent(JsonWin, System.Text.Encoding.UTF8, "application/json");
            var JsonLose = Newtonsoft.Json.JsonConvert.SerializeObject(updatedHistoricL);
            var ContentL = new StringContent(JsonLose, System.Text.Encoding.UTF8, "application/json");
            string combinedUrlW = Path.Combine(EndpointsDictionary["Players"], IdPlayerWin);
            string combinedUrll = Path.Combine(EndpointsDictionary["Players"], IdPlayerLose);
            Tools.Put(combinedUrlW, ContentW);
            Tools.Put(combinedUrll, ContentL);
        }
    }
}


















