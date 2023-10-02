
namespace Verifications
{
    internal class Dictionaries
    {
        public Dictionaries(){}
        public string GetMappedString(string input)
        {
            if (MapsDictionary.TryGetValue(input, out string mapName))
            {
                return mapName;
            }
            else
            {
                return null;
            }
        }
        public string GetMapIDstring(string input)
        {
            if (MapsIdDictionary.TryGetValue(input, out string MapID))
            {
                return MapID;
            }
            else
            {
                return null;
            }
        }
        public string GetPlayerString(string input)
        {
            if (PlayersDictionary.TryGetValue(input, out string PlayerName))
            {
                return PlayerName;
            }
            else
            {
                return null;
            }
        }
        public string GetFactionIDstring(string input)
        {
            if (FactionDictionary.TryGetValue(input, out string factionID))
            {
                return factionID;
            }
            else
            {
                return null;
            }
        }
        public string GetVersion(ushort input)
        {
            if (VersionsDictionary.TryGetValue(input, out string version))
            {
                return version;
            }
            else
            {
                return null;
            }
        }
        public List<string> FetchNicknames (string key)
        {
             var matchingKeys = PlayersDictionary
                   .Where(kvp => kvp.Value == key)
                   .Select(kvp => kvp.Key)
                   .ToList();
            return matchingKeys;
        }
        public static Dictionary<string, string> MapsDictionary = new Dictionary<string, string>()
        {
            {"jungle of far harad", "Jungles of Far Harad"},
            {"sakura forest", "Sakura Forest"},
            {"westfold", "Westfold"},
            {"ettenmoors", "Ettenmoors"},
            {"brandy_hills", "Brandy Hills"},
            {"chetwood", "Chetwood"},
            {"sorrow isles", "Sorow Isles"},
            {"belfala abyss", "Belfalas Abyss"},
            {"eastfold", "Eastfold II"},
            {"rhun shore", "Rhun Shore"},
            {"blood gulch", "Blood Gulch"},
            {"cirdan_ cove", "Cirdan's Cove"},
            {"dunland_ii", "Dunland II"},
            {"erech iii", "Erech 4th Age"},
            {"eryn laer", "Eryn Laer"},
            {"fangorn", "Fangorn"},
            {"firien dale", "Firien Dale"},
            {"ford of bruinen", "Fords Of Bruinen"},
            {"ford of isen", "Fords Of Isen"},
            {"gap of rohan", "Gap of Rohan"},
            {"glenford", "Glenford"},
            {"hollin", "Hollin"},
            {"last_bridge", "Last Bridge"},
            {"lossarnach", "Lossarnach"},
            {"mering stream", "Mering Stream"},
            {"mirkwood forest", "Mirkwood II"},
            {"plain of lindon", "Plains of Lindon"},
            {"sirannon", "Sirannon"},
            {"iron hills", "The Iron Hills"},
            {"tomb of karna", "Tombs of Karna"},
            {"tyrn gorthad", "Tyrn Gorthad"},
            {"ettenmoor ii", "Ettenmoors II"},
            {"rhudaur", "Rhudaur"},
            {"midgewater", "Midgewater"}, //bywater?
            {"ford of isen (unlame)", "Fords of Isen (unlame)"},
            {"error","error" }
        };
        public static Dictionary<string, string> MapsIdDictionary = new Dictionary<string, string>()
        {
            {"Jungles of Far Harad","1"},
            {"Sakura Forest","2"},
            {"Westfold","3"},
            {"Ettenmoors","4"},
            {"Brandy Hills","5"},
            {"Chetwood","6"},
            {"Sorow Isles","7"},
            { "Belfalas Abyss","8"},
            { "Eastfold II","9"},
            { "Rhun Shore","10"},
            { "Blood Gulch","12"},
            { "Cirdan's Cove","13"},
            { "Dunland II","14"},
            { "Erech 4th Age","15"},
            { "Eryn Laer","16"},
            {"Fangorn","17"},
            { "Firien Dale","18"},
            { "Fords Of Bruinen","19"},
            { "Fords Of Isen","20"},
            { "Gap of Rohan","21"},
            { "Glenford","22"},
            { "Hollin","23"},
            { "Last Bridge","24"},
            { "Lossarnach","25"},
            { "Mering Stream","26"},
            { "Mirkwood II","27"},
            { "Plains of Lindon","28"},
            { "Sirannon","29"},
            { "The Iron Hills","30"},
            { "Tombs of Karna","31"},
            { "Tyrn Gorthad","32"},
            { "Ettenmoors II","33"},
            { "Rhudaur","34"},
            { "Midgewater","35"}, //bywater?
            { "Fords of Isen (unlame)","36"},
            {"error","error" }
        };
        public static Dictionary<string, string> FactionDictionary = new Dictionary<string, string>()
        {
           {"Men", "1"},
           {"Isengard", "2"},
           {"Angmar", "3"},
           {"Mordor", "4"},
           {"Elves", "5"},
           {"Goblins", "6"},
           {"Dwarves", "7"}
        };
        public static Dictionary<string, string> PlayersDictionary = new Dictionary<string, string>()
        {
            {"KIMG.MUSTA", "KING MUSTAFA"},
            {"papi", "Papi Chullo"},
            {"Mex_Flaxtz", "Mex Flaxtz"},
            {"Mira`", "Miraculix"},
            {"MayShadowF", "MayShadowFax"},
            {"M__", "Tom-Exp"},

            {"Ara", "Artanis"},
            {"Zeratul", "Artanis"},
            {"Aratnis", "Artanis"},
            //theese are tests
            //{"FipuA", "SOLID | Aupif"  },
            //{ "RuSh|Wyrda", "wyr"}
        };
        public static Dictionary <ushort, string> VersionsDictionary = new Dictionary<ushort, string>()
        {
           {6970, "9000"},
           {56951, "9001"},
           {22858, "8400"},
           {24451, "some beta" },
           //{24451, "some beta" }
        };
    }
}

