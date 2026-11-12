public static class BankHelper
{
    public static bool IsValidAccountNumber(string accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            return false;
        }

        string normalized = accountNumber.Trim();
        return normalized.Length is >= 4 and <= 26 &&
               normalized.All(character => character is >= '0' and <= '9');
    }

    public static string GenerateSampleAccountNumber(int sequence) =>
        sequence is >= 1 and <= 99999999
            ? $"{sequence:00000000}"
            : throw new ArgumentOutOfRangeException(nameof(sequence));

    public static string GetBankInformation() =>
        "Bank Demo - system obsługi kont oszczędnościowych, firmowych i osobistych.";
}
