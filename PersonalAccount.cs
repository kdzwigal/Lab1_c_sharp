public sealed class PersonalAccount : BankAccount
{
    public PersonalAccount(
        string accountNumber,
        Customer owner,
        decimal balance,
        string currency,
        int freeTransfersPerMonth)
        : base(accountNumber, owner, balance, currency, "Personal")
    {
        if (freeTransfersPerMonth < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(freeTransfersPerMonth));
        }

        FreeTransfersPerMonth = freeTransfersPerMonth;
    }

    public int FreeTransfersPerMonth { get; }

    public int TransfersUsedThisMonth { get; private set; }

    protected override decimal CalculateTransferFee(decimal amount)
    {
        TransfersUsedThisMonth++;
        return TransfersUsedThisMonth > FreeTransfersPerMonth ? 2m : 0m;
    }

    public override decimal CalculateMonthlyFee() => 0m;
}
