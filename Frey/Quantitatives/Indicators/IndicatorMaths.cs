namespace Automata.Quantitatives.Indicators
{
    public static class IndicatorMaths
    {
        public static double CalculateEMA(double input, int periods, double lastEMAValue)
        {
            var periodFactor = 2 / (double)(periods + 1);
            return input * periodFactor + lastEMAValue * (1 - periodFactor);
        }

        public static double CalculateEMA(double input, double periodFactor, double lastEMAValue)
        {
            return input * periodFactor + lastEMAValue * (1 - periodFactor);
        }
    }
}