using ReplayDataParse;

namespace FileParse
{
    public class ExtractReplayData
    {
        readonly ReplayData data;
        readonly Palyer Player1Data;
        readonly Palyer Player2Data;
        readonly string OwnerName;
        readonly List<string> ObserverList;

        public ExtractReplayData(Stream FileStream)
        {
            ReplayParser file1 = new ReplayParser(FileStream);
            data = file1.Parse();
            OwnerName = data.HeaderFields.Players[Convert.ToInt32(data.ReplaySaver)].Name;
            ObserverList = new List<string>();

            for (int i = 0; i < data.HeaderFields.Players.Count; i++)
            {
                if (data.HeaderFields.Players[i].Faction == -2)
                {
                    ObserverList.Add(data.HeaderFields.Players[i].Name);
                }
                if (data.HeaderFields.Players[i].Faction != -2 && Player1Data == null)
                {
                    Player1Data = data.HeaderFields.Players[i];
                    continue;
                }
                if (data.HeaderFields.Players[i].Faction != -2 && Player2Data == null)
                {
                    Player2Data = data.HeaderFields.Players[i];
                }
            }
        }

        public DateTime GetDateTimeReplayStarted()
        {
            return DateTimeOffset.FromUnixTimeSeconds(data.TimestampStart).UtcDateTime;
        }

        public string GetDateReplayStarte()
        {
            DateTime date = DateTimeOffset.FromUnixTimeSeconds(data.TimestampStart).UtcDateTime;
            return date.ToShortDateString();
        }

        public string GetTimeReplayStarte()
        {
            DateTime date = DateTimeOffset.FromUnixTimeSeconds(data.TimestampStart).UtcDateTime;
            return date.ToShortTimeString();
        }

        public DateTime GetDateTimeReplayEnded()
        {
            return DateTimeOffset.FromUnixTimeSeconds(data.TimestampEnd).UtcDateTime;
        }

        public string GetDateReplayEnded()
        {
            DateTime date = DateTimeOffset.FromUnixTimeSeconds(data.TimestampEnd).UtcDateTime;
            return date.ToShortDateString();
        }

        public string GetTimeReplayEnded()
        {
            DateTime date = DateTimeOffset.FromUnixTimeSeconds(data.TimestampEnd).UtcDateTime;
            return date.ToShortTimeString();
        }

        public string GetMap()
        {
            string mapname = data.HeaderFields.MapName;
            mapname = mapname.Remove(0, 9);
            mapname = mapname.Replace("/", " ");
            mapname = mapname.Replace("s ", " ");
            mapname = mapname.Replace("map et ", " ");
            mapname = mapname.Replace("map tt ", " ");
            mapname = mapname.Replace("map_pk_", " ");
            mapname = mapname.Replace("map sb ", " ");
            mapname = mapname.TrimStart();
            return mapname;
        }

        public string GetPlayer1Name()
        {
            return Player1Data.Name;
        }

        public string GetPlayer1Faction()
        {
            return Player1Data.faction;
        }

        public string GetPlayer2Name()
        {
                try
                {
                      if (Player2Data != null)
                      {
                          return Player2Data.Name;
                      }
                      else
                      {
                          return "error";
                      }
                }
                catch (Exception ex)
                {
                    return "error";
                }
        }

        public string GetPlayer2Faction()
        {
                try
                {
                if (Player2Data != null)
                {
                    return Player2Data.faction;
                }
                else
                {
                    return "error";
                }
                }
                catch (Exception ex)
                {
                    return "error";
                }
        }

        public List<string> GetObservers()
        {
            return ObserverList;
        }

        public string GetReplayOwner()
        {
            return OwnerName;
        }

        public ushort GetGameVersion()
        {
            return data.VersionMajor;
        }
    }
}
