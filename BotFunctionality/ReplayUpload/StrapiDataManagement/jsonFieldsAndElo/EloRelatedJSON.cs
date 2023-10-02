using Newtonsoft.Json.Linq;

namespace jsonFieldsAndElo
{
    internal class EloRelatedJSON
    {
        private CalculateElo Calculator;
        private (int deltaA, int ratingA, int deltaB, int ratingB) calculations;
        private string EloField;
        //private string HistoricField;

        public EloRelatedJSON(int win_id, string win_name, int win_eloBefore, int lose_id, string lose_name, int lose_eloBefore)
        {
            Calculate(win_eloBefore, lose_eloBefore);

            var Form = new
            {
                player_win = new
                {
                    id = win_id,
                    name = win_name,
                    elo_win = GetResultRatingWinner(),
                    before = win_eloBefore,
                    gain = calculations.deltaA
                },
                player_lose = new
                {
                    id = lose_id,
                    name = lose_name,
                    elo_lose = GetResultRatingLoser(),
                    before = lose_eloBefore,
                    loss = calculations.deltaB
                },
            };
            EloField = Newtonsoft.Json.JsonConvert.SerializeObject(Form);
        }

        private void Calculate(int win_eloBefore, int lose_eloBefore)
        {
            Calculator = new CalculateElo();
            if (win_eloBefore - lose_eloBefore <= -200)
            {
                calculations = Calculator.GetOutcome(win_eloBefore, lose_eloBefore, 1, 80, 200);
            }
            else if (win_eloBefore - lose_eloBefore >= 200)
            {
                calculations = Calculator.GetOutcome(win_eloBefore, lose_eloBefore, 1, 10, 800);
            }
            else
            {
                calculations = Calculator.GetOutcome(win_eloBefore, lose_eloBefore, 1, 50, 800);
            }
        }

        public string GetReplayEloField()
        {
            return EloField;
        }

        public int GetResultRatingWinner()
        {
            return calculations.ratingA;
        }

        public int GetResultRatingLoser()
        {
            return calculations.ratingB;
        }

        public JArray UpdateHistoricFieldWinner(JArray historic)
        {
            return HistoricUpdater(historic, calculations.deltaA);
        }

        public JArray UpdateHistoricFieldLoser(JArray historic)
        {

            return HistoricUpdater(historic, calculations.deltaB);
        }

        private JArray HistoricUpdater(JArray historic, int delta)
        {
            if (historic.Count > 0)
            {
                var newBlock = new JObject
                {
                    { "before", (int)historic.Last["before"] + (int)historic.Last["resulat"] },
                    { "resulat",  delta }
                };

                historic.Add(newBlock);
                return historic;
            }

            var newBlockDefault = new JObject
            {
                    { "before", 1000 },
                    { "resulat",  delta }
            };

            JArray created = new JArray(historic);
            created.Add(newBlockDefault);
            return created;
        }
    }
}
