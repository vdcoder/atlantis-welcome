namespace Atlantis.Api.Economy.Currency;

public static class AtlantisCurrency
{
    public const string Code = "ATC";

    public const int DecimalPlaces = 2;

    public static decimal Normalize(decimal amount)
    {
        return decimal.Round(
            amount,
            DecimalPlaces,
            MidpointRounding.ToEven);
    }

    public static bool IsValidAmount(decimal amount)
    {
        return amount == Normalize(amount);
    }
}