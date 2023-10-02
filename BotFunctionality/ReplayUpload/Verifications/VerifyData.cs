
namespace Verifications
{
    internal class VerifyData
    {
        private string map;
        private ushort version;
        private string Player1Name;
        private string Player2Name;
        private string Faction1Name;
        private string Faction2Name;
        private List<string> ListOfNicks;
        //private List<string> ListOfMapNames;
        private Dictionaries Checkings;

        public VerifyData(string name1, string name2, string mapname, List<string> listOfNicksFromStrapi, string Faction1, string Faction2, ushort Version)
        {
            Checkings = new Dictionaries();
            ListOfNicks = listOfNicksFromStrapi;
            Player1Name = name1;
            Player2Name = name2;
            Faction1Name = Faction1;
            Faction2Name = Faction2;
            map = mapname;
            version = Version;
        }

        public (string, string, string, string, string, string, string) PerformVerification()
        {
            (string, string, string, string, string, string, string) result = ("error", "error", "error", "error", "error", "error", "error");
            
            string a = Checkings.GetMappedString(map);  //4
            string b = Checkings.GetPlayerString(Player1Name);
            string c = Checkings.GetPlayerString(Player2Name);

            if (b == null)
            {
                b = Verify(ListOfNicks, Player1Name);  //2
            }
            if (c == null)
            {
                c = Verify(ListOfNicks, Player2Name);  //2
            }
            if (a != null)
            {
                result.Item1 = a;
                result.Item2 = Checkings.GetMapIDstring(a);
            }
            if (b != null)
            {
                result.Item3 = b;
            }
            if (c != null)
            {
                result.Item4 = c;
            }

            result.Item5 = Checkings.GetFactionIDstring(Faction1Name);
            result.Item6 = Checkings.GetFactionIDstring(Faction2Name);
            result.Item7 = Checkings.GetVersion(version);
            return result;
        }

        private string Verify(List<string> theList, string name)
        {
            const double minSimilarityThreshold = 0.75; // Adjust this threshold as needed
            foreach (string item in theList)
            {
                double similarity = CalculateJaccardSimilarity(item, name);
                if (similarity >= minSimilarityThreshold)
                {
                    return item;
                }
            }
            return null;
        }

        private double CalculateJaccardSimilarity(string a, string b)
        {
            var setA = new HashSet<char>(a.ToLower());
            var setB = new HashSet<char>(b.ToLower());
            var intersection = setA.Intersect(setB).Count();
            var union = setA.Union(setB).Count();
            return (double)intersection / union;
        }
    }
}
