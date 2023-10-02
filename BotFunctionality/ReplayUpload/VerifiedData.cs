
namespace ReplayUpload
{
    public class VerifiedData
    {

        public VerifiedData() { }

        public string Map { get; set; }
        public string MapID { get; set; }
        public string Player1 { get; set; }
        public string Player2 { get; set; }
        public string Faction1ID { get; set; }
        public string Faction2ID { get; set; }
        public string OwnerName { get; set; }
        public List<string> ObserverList { get; set; }
        public string Version { get; set; }
        public string FileName { get; set; }
        public bool Swap { get; set; }


        public DateTime DateTimeReplayStarted { get; set; }
        public string DateReplayStarted { get; set; }
        public string TimeReplayStarted { get; set; }
        public DateTime DateTimeReplayEnded { get; set; }
        public string DateReplayEnded { get; set; }
        public string TimeReplayEnded { get; set; }
        public TimeSpan ReplayDuration => DateTimeReplayEnded - DateTimeReplayStarted;
    }

    public class FileAndVerifiedDataPair
    {
        public FileAndVerifiedDataPair () {}
        public Stream File { get; set; }
        public string FileName { get; set; }
        public VerifiedData VerifiedData { get; set; }
    }
}
