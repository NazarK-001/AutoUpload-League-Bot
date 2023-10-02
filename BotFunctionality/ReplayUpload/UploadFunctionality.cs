using FileParse;
using Newtonsoft.Json.Linq;
using StrapiDataManagement;
using Verifications;

namespace ReplayUpload
{
    public class UploadFunctionality
    {
        private List<FileAndVerifiedDataPair> Data;
        private List<FileAndVerifiedDataPair> LastData;
        private PostingStrapiData PostingTool;

        public UploadFunctionality()
        {
            Data = new List<FileAndVerifiedDataPair>();
            LastData = new List<FileAndVerifiedDataPair>();
            PostingTool = new PostingStrapiData();
        }
        public void AddMoreData(List<Stream> filesCollection, List<string> fileNames)
        {
            ProcessData(filesCollection, fileNames);
            OrderByTimeEnded();

        }
        private void ProcessData(List<Stream> filesCollection, List<string> fileNames)
        {
            // Cycle for all the files
            for (int i = 0; i < filesCollection.Count; i++)
            {
                // Getting the latest info about players from Strapi
                GettingEntireData EntireData = new GettingEntireData();
                List<string> ListOfNicks = EntireData.GetListOfNicks();

                // Now getting info from the replay file
                ExtractReplayData ReplayData = new ExtractReplayData(filesCollection[i]);
                string Player1Name = ReplayData.GetPlayer1Name();
                string Player2Name = ReplayData.GetPlayer2Name();
                string Faction1Name = ReplayData.GetPlayer1Faction();
                string Faction2Name = ReplayData.GetPlayer2Faction();
                string map = ReplayData.GetMap();
                ushort version = ReplayData.GetGameVersion();
                string ownername = ReplayData.GetReplayOwner();
                List<string> observerlist = ReplayData.GetObservers();


                // making the winner decision
                VerifyData verification;
                char resultIndicator = char.ToLower(fileNames[i][0]);
                string winnerName = (Player1Name == ownername && resultIndicator == 'w') ||
                                    (Player2Name == ownername && resultIndicator == 'l')
                                    ? Player1Name
                                    : Player2Name;
                string loserName = (winnerName == Player1Name) ? Player2Name : Player1Name;
                string winnerFaction = (winnerName == Player1Name) ? Faction1Name : Faction2Name;
                string loserFaction = (winnerFaction == Faction1Name) ? Faction2Name : Faction1Name;
                Console.WriteLine($"winnerName is {winnerName}");
                Console.WriteLine($"loserName is {loserName}");
                verification = new VerifyData(winnerName, loserName, map, ListOfNicks, winnerFaction, loserFaction, version);
                bool swap = (winnerName == Player1Name) ? false : true;

                // Verify the data extracted from the file. Also get back the verified data
                (string map, string mapID, string player1, string player2, string faction1ID, string faction2ID, string Version) verifiedData = verification.PerformVerification();

                // Write down all necesary data
                VerifiedData verify = new VerifiedData
                {
                    Map = verifiedData.map,
                    MapID = verifiedData.mapID,
                    Player1 = verifiedData.player1,
                    Player2 = verifiedData.player2,
                    Faction1ID = verifiedData.faction1ID,
                    Faction2ID = verifiedData.faction2ID,
                    OwnerName = ownername,
                    ObserverList = observerlist,
                    Version = verifiedData.Version,
                    FileName = fileNames[i],
                    Swap = swap,

                    DateTimeReplayStarted = ReplayData.GetDateTimeReplayStarted(),
                    DateReplayStarted = ReplayData.GetDateReplayStarte(),
                    TimeReplayStarted = ReplayData.GetTimeReplayStarte(),
                    DateTimeReplayEnded = ReplayData.GetDateTimeReplayEnded(),
                    DateReplayEnded = ReplayData.GetDateReplayEnded(),
                    TimeReplayEnded = ReplayData.GetTimeReplayEnded()
                };

                FileAndVerifiedDataPair fileAndVerifiedDataPair = new FileAndVerifiedDataPair
                {
                    File = filesCollection[i],
                    FileName = fileNames[i],
                    VerifiedData = verify
                };

                LastData.Add(fileAndVerifiedDataPair);

                if (observerlist.Contains(ownername))
                {
                    verify.OwnerName = "error";
                }
            }
        }

        private void OrderByTimeEnded()
        {
            LastData.Sort((info1, info2) =>
            info2.VerifiedData.DateTimeReplayEnded.CompareTo(info1.VerifiedData.DateTimeReplayEnded));
            LastData.Reverse();
            Data.AddRange(LastData);
        }

        public List<VerifiedData> GetLastVerifiedDataList() 
        {
            var result = LastData.Select(pair => pair.VerifiedData).ToList();
            LastData.Clear();
            return result;
        }

        public async Task UploadData(int FileIndex)
        {
            // Getting the latest info about players from Strapi
            GettingEntireData EntireData = new GettingEntireData();
            string PlayersResponse = EntireData.GetPlayersResponse();
            ParsingPlayerStrapiData PlayerDataTool = new ParsingPlayerStrapiData(PlayersResponse);

            // upload the file and get its id
            string FileID = await PostingTool.NewFileUpload(Data[FileIndex].File, Data[FileIndex].FileName);

            //get the current players elo
            int elo1 = PlayerDataTool.GetPlayersElo(Data[FileIndex].VerifiedData.Player1);
            int elo2 = PlayerDataTool.GetPlayersElo(Data[FileIndex].VerifiedData.Player2);

            // getting the date start and date end in correct format
            string DateStart = Data[FileIndex].VerifiedData.DateTimeReplayStarted.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            string DateEnd = Data[FileIndex].VerifiedData.DateTimeReplayEnded.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            // get id of those players that participated in the match
            string idplayer1 = PlayerDataTool.GetPlayersID(Data[FileIndex].VerifiedData.Player1);
            string idplayer2 = PlayerDataTool.GetPlayersID(Data[FileIndex].VerifiedData.Player2);

            // gather all the data and create a new game entry on strapi
            PostingTool.NewGameEntry(DateStart, DateEnd, Data[FileIndex].VerifiedData.Faction1ID, Data[FileIndex].VerifiedData.Faction2ID, idplayer1, idplayer2, FileID, Data[FileIndex].VerifiedData.MapID, Data[FileIndex].VerifiedData.Player1, Data[FileIndex].VerifiedData.Player2, elo1, elo2);

            // get the current historics of those players
            JArray historic1 = PlayerDataTool.GetPlayersHistoric(Data[FileIndex].VerifiedData.Player1);
            JArray historic2 = PlayerDataTool.GetPlayersHistoric(Data[FileIndex].VerifiedData.Player2);

            // modify the players parameters on strapi
            PostingTool.ModifyPlayer(historic1, historic2);
        }

        public List<string> FetchNicknames(string key)
        {
            Dictionaries fetch = new Dictionaries();
            return fetch.FetchNicknames(key);
        }
    }
}



