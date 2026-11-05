public class BankAccount
{
    public BankAccount(
        string accountNumber,
        Customer owner,
        decimal balance,
        string currency,
        string accountType)
    {
        AccountNumber = RequireText(accountNumber, nameof(accountNumber));
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

    public void Deposit(decimal amount)
    {
        EnsureActive();
        ValidatePositiveAmount(amount);

        Balance += amount;
        Transactions.Add(new Transaction(
            TransactionType.Deposit,
            amount,
            Currency,
            "Wpłata środków",
            destinationAccountNumber: AccountNumber));
    }

    public void Withdraw(decimal amount)
    {
        EnsureActive();
        ValidatePositiveAmount(amount);

        if (amount > Balance)
        {
            throw new InvalidOperationException("There are insufficient funds for this withdrawal.");
        }

        Balance -= amount;
        Transactions.Add(new Transaction(
            TransactionType.Withdrawal,
            amount,
            Currency,
            "Wypłata środków",
            sourceAccountNumber: AccountNumber));
    }

    public void TransferTo(BankAccount recipient, decimal amount)
    {
        ArgumentNullException.ThrowIfNull(recipient);
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

        if (amount > Balance)
        {
            throw new InvalidOperationException("There are insufficient funds for this transfer.");
        }

        Balance -= amount;
        recipient.Balance += amount;

        Transactions.Add(new Transaction(
            TransactionType.TransferOut,
            amount,
            Currency,
            $"Przelew do konta {recipient.AccountNumber}",
            sourceAccountNumber: AccountNumber,
            destinationAccountNumber: recipient.AccountNumber));

        recipient.Transactions.Add(new Transaction(
            TransactionType.TransferIn,
            amount,
            Currency,
            $"Przelew z konta {AccountNumber}",
            sourceAccountNumber: AccountNumber,
            destinationAccountNumber: recipient.AccountNumber));
    }

    public string GetAccountSummary()
    {
        return $"{AccountNumber} | {Owner.FullName} | {AccountType} | " +
               $"{Balance:F2} {Currency} | {(IsActive ? "Aktywne" : "Nieaktywne")}";
    }

    public string GetCurrentBalance()
    {
        return $"{Balance:F2} {Currency}";
    }

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

    private void EnsureActive()
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("The account is inactive.");
        }
    }

    private static decimal ValidateInitialBalance(decimal balance)
    {
        if (balance < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(balance), "Balance cannot be negative.");
        }

        return balance;
    }

    private static void ValidatePositiveAmount(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        }
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
}
