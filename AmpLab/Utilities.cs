namespace AmpLab;

public static class Utilities
{
    // Bardzo ważna metoda, która pozwala na parsowanie danych wejściowych xD
    public static double ParseInput(string input)
    {
        return double.TryParse(input, out var result) ? result : 0;
    }
}