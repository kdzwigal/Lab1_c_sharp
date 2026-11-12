public sealed class BusinessAccount : BankAccount
{
    public BusinessAccount(
        string accountNumber,
        Customer owner,
        decimal balance,
        string currency,
        string taxId,
        decimal transferCommissionRate)
        : base(accountNumber, owner, balance, currency, "Business")
    {
        TaxId = RequireText(taxId, nameof(taxId));
        if (transferCommissionRate < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(transferCommissionRate));
        }

        TransferCommissionRate = transferCommissionRate;
    }

    public string TaxId { get; }

    public decimal TransferCommissionRate { get; }

    protected override decimal CalculateTransferFee(decimal amount) =>
        amount * TransferCommissionRate;

    public override decimal CalculateMonthlyFee() => 25m;

    private static string RequireText(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be empty or whitespace.", parameterName);
        }

        return value.Trim();
    }
}
