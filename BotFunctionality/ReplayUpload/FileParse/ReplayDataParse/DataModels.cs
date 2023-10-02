
namespace ReplayDataParse
{
    internal class ReplayData
    {
        public uint TimestampStart;
        public uint TimestampEnd;
        public string HeaderRawData;
        public HeaderFields HeaderFields;
        public string ReplaySaver;
        public ushort VersionMajor;
    }
    internal class Palyer
    {
        public string Name;
        public string faction;
        public int Faction;
        public int Spot;
    }
    internal class HeaderFields
    {
        public string MapName; // M=
        public string Seed; // SD=
        //public uint TimestampEnd;
        public List<Palyer> Players;
    }
    internal class RotwkFaction
    {
        private static readonly Dictionary<int, string> _factions = new Dictionary<int, string>
        {
            {-2, "Observer"},//Observer
            {-1, "Random"},//Random
            {3, "Men"},//Men
            {5, "Elves"},//Elves
            {6, "Dwarves"},//Dwarves
            {7, "Isengard"},//Isengard
            {8, "Mordor"},//Mordor
            {9, "Goblins"},//Goblins
            {10, "Angmar"} //Angmar
        };

        private int _identifier;

        private RotwkFaction(int id)
        {
            _identifier = id;
        }
        public static RotwkFaction FromName(string factionName)
        {
            var faction = new RotwkFaction(_factions.FirstOrDefault(x => x.Value.Contains(factionName)).Key);
            return faction;
        }

        public static RotwkFaction FromReplayID(int factionId)
        {
            var faction = new RotwkFaction(factionId);
            return faction;
        }

        public static RotwkFaction FromRandom(int factionId)
        {
            var i = 0;
            foreach (var item in GetPlayableFactions())
            {
                if (factionId == i)
                {
                    return new RotwkFaction(item.Key);
                }
                i++;
            }
            return null;
        }

        public static int GetNumberOfPlayableFactions()
        {
            return GetPlayableFactions().Count();
        }

        private static IEnumerable<KeyValuePair<int, string>> GetPlayableFactions()
        {
            return _factions.Where(pair => pair.Key > 0);
        }

        public int GetRawIdentifier()
        {
            return _identifier;
        }

        public bool IsObserver()
        {
            return FromName("Observer").GetRawIdentifier() == _identifier;
        }

        public bool IsRandom()
        {
            return FromName("Random").GetRawIdentifier() == _identifier;
        }

        public bool IsPlayable()
        {
            return _identifier > 0;
        }

        public override string ToString()
        {
            return _factions[_identifier];
        }
    }
}
