using System.Text.Json.Serialization;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(SavingsAccount), "savings")]
[JsonDerivedType(typeof(BusinessAccount), "business")]
[JsonDerivedType(typeof(PersonalAccount), "personal")]
public abstract class BankAccount
{
    protected BankAccount(
        string accountNumber,
        Customer owner,
        decimal balance,
        string currency,
        string accountType)
    {
        if (!BankHelper.IsValidAccountNumber(accountNumber))
        {
            throw new ArgumentException("Account number must contain 4 to 26 digits.", nameof(accountNumber));
        }

        AccountNumber = accountNumber.Trim();
        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        Balance = ValidateInitialBalance(balance);
        Currency = ValidateCurrency(currency);
        AccountType = RequireText(accountType, nameof(accountType));
        IsActive = true;
    }

    public string AccountNumber { get; }

    public Customer Owner { get; }

    public decimal Balance { get; private set; }

    public string Currency { get; }

    public bool IsActive { get; private set; }

    public string AccountType { get; }

    public List<Transaction> Transactions { get; } = new();

    public abstract decimal CalculateMonthlyFee();

    public virtual void ApplyMonthlyFee() => ChargeMonthlyFee();

    public virtual void Deposit(decimal amount)
    {
        EnsureActive();
        ValidatePositiveAmount(amount);
        Balance += amount;
        AddTransaction(TransactionType.Deposit, amount, "Wpłata środków", destinationAccountNumber: AccountNumber);
    }

    public virtual void Withdraw(decimal amount)
    {
        EnsureActive();
        ValidatePositiveAmount(amount);
        EnsureSufficientFunds(amount, "There are insufficient funds for this withdrawal.");
        Balance -= amount;
        AddTransaction(TransactionType.Withdrawal, amount, "Wypłata środków", sourceAccountNumber: AccountNumber);
    }

    public virtual void TransferTo(BankAccount recipient, decimal amount)
    {
        ArgumentNullException.ThrowIfNull(recipient);
        EnsureTransferIsPossible(recipient, amount);

        decimal fee = CalculateTransferFee(amount);
        EnsureSufficientFunds(amount + fee, "There are insufficient funds for this transfer and its fee.");
        Balance -= amount + fee;
        recipient.Balance += amount;

        AddTransaction(
            TransactionType.TransferOut,
            amount,
            $"Przelew do konta {recipient.AccountNumber}",
            sourceAccountNumber: AccountNumber,
            destinationAccountNumber: recipient.AccountNumber);
        recipient.AddTransaction(
            TransactionType.TransferIn,
            amount,
            $"Przelew z konta {AccountNumber}",
            sourceAccountNumber: AccountNumber,
            destinationAccountNumber: recipient.AccountNumber);

        if (fee > 0)
        {
            AddTransaction(TransactionType.Fee, fee, "Prowizja za przelew", sourceAccountNumber: AccountNumber);
        }
    }

    protected virtual decimal CalculateTransferFee(decimal amount) => 0m;

    protected void ChargeMonthlyFee()
    {
        decimal fee = CalculateMonthlyFee();
        if (fee <= 0)
        {
            return;
        }

        EnsureSufficientFunds(fee, "There are insufficient funds for the monthly fee.");
        Balance -= fee;
        AddTransaction(TransactionType.Fee, fee, "Miesięczna opłata za prowadzenie konta", sourceAccountNumber: AccountNumber);
    }

    protected void AddInterest(decimal amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Balance += amount;
        AddTransaction(TransactionType.Interest, amount, "Naliczono odsetki", destinationAccountNumber: AccountNumber);
    }

    protected void EnsureActive()
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("The account is inactive.");
        }
    }

    protected static void ValidatePositiveAmount(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        }
    }

    private void EnsureTransferIsPossible(BankAccount recipient, decimal amount)
    {
        EnsureActive();
        recipient.EnsureActive();
        ValidatePositiveAmount(amount);

        if (ReferenceEquals(this, recipient))
        {
            throw new ArgumentException("An account cannot transfer money to itself.", nameof(recipient));
        }

        if (!string.Equals(Currency, recipient.Currency, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Transfers require both accounts to use the same currency.");
        }
    }

    private void EnsureSufficientFunds(decimal amount, string message)
    {
        if (amount > Balance)
        {
            throw new InvalidOperationException(message);
        }
    }

    private void AddTransaction(
        TransactionType type,
        decimal amount,
        string description,
        string? sourceAccountNumber = null,
        string? destinationAccountNumber = null)
    {
        Transactions.Add(new Transaction(
            type,
            amount,
            Currency,
            description,
            sourceAccountNumber,
            destinationAccountNumber));
    }

    private static decimal ValidateInitialBalance(decimal balance)
    {
        if (balance < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(balance), "Balance cannot be negative.");
        }

        return balance;
    }

    private static string ValidateCurrency(string currency)
    {
        string normalizedCurrency = RequireText(currency, nameof(currency)).ToUpperInvariant();
        if (normalizedCurrency.Length != 3 || normalizedCurrency.Any(character => character is < 'A' or > 'Z'))
        {
            throw new ArgumentException("Currency must be a three-letter code.", nameof(currency));
        }

        return normalizedCurrency;
    }

    private static string RequireText(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be empty or whitespace.", parameterName);
        }

        return value.Trim();
    }

    public string GetAccountSummary() =>
        $"{AccountNumber} | {Owner.FullName} | {AccountType} | " +
        $"{Balance:F2} {Currency} | {(IsActive ? "Aktywne" : "Nieaktywne")}";

    public string GetCurrentBalance() => $"{Balance:F2} {Currency}";

    public void DisplayTransactionHistory()
    {
        if (Transactions.Count == 0)
        {
            Console.WriteLine("Brak transakcji.");
            return;
        }

        foreach (Transaction transaction in Transactions)
        {
            Console.WriteLine(
                $"{transaction.OccurredAt:yyyy-MM-dd HH:mm:ss} | " +
                $"{transaction.Type,-11} | {transaction.Amount:F2} {transaction.Currency} | " +
                transaction.Description);
        }
    }
}
