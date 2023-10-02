
namespace jsonFieldsAndElo
{
    internal class CalculateElo
    {
        private double k;
        private double n;

        public CalculateElo(double k = 24, double n = 400)
        {
            this.k = k;
            this.n = n;
        }

        public double GetExpectedScore(double ratingA, double ratingB, double n = 0)
        {
            n = n == 0 ? this.n : n;
            return 1 / (1 + Math.Pow(10, (ratingB - ratingA) / n));
        }

        public double GetRatingDelta(double ratingA, double ratingB, double score, double k = 0, double n = 0)
        {
            k = k == 0 ? this.k : k;
            double expectedScore = GetExpectedScore(ratingA, ratingB, n);
            return k * (score - expectedScore);
        }

        public double GetRating(double ratingA, double ratingB, double score, double k = 0, double n = 0)
        {
            double delta = GetRatingDelta(ratingA, ratingB, score, k, n);
            return ratingA + delta;
        }

        public (int deltaA, int ratingA, int deltaB, int ratingB) GetOutcome(double ratingA, double ratingB, double score, double k = 0, double n = 0)
        {
            double delta = GetRatingDelta(ratingA, ratingB, score, k, n);
            return (
                deltaA: RoundTheNumber(delta),
                ratingA: RoundTheNumber(ratingA + delta),
                deltaB: RoundTheNumber(-delta),
                ratingB: RoundTheNumber(ratingB - delta)
            );
        }

        private int RoundTheNumber(double number)
        {
            double integerPart = Math.Floor(number);
            if (number % 1 != 0)
            {
                integerPart++;
            }
            return (int)integerPart;
        }
    }
}
