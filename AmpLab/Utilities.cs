namespace AmpLab
{
    public static class Utilities
    {
        public static double ParseInput(string input)
        {
            return double.TryParse(input, out double result) ? result : 0;
        }
    }
}