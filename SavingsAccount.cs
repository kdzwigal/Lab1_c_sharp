public sealed class SavingsAccount : BankAccount
{
    public SavingsAccount(
        string accountNumber,
        Customer owner,
        decimal balance,
        string currency,
        decimal annualInterestRate)
        : base(accountNumber, owner, balance, currency, "Savings")
    {
        if (annualInterestRate < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(annualInterestRate));
        }

        AnnualInterestRate = annualInterestRate;
    }

    public decimal AnnualInterestRate { get; }

    public void ApplyMonthlyInterest()
    {
        EnsureActive();
        AddInterest(Balance * AnnualInterestRate / 12m);
    }

    public override decimal CalculateMonthlyFee() => 5m;
}
