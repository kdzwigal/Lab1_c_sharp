using System;

public class BankAccount
{
    private string _accountNumber = string.Empty;
    private string _ownerName = string.Empty;
    private decimal _balance;
    private string _currency = string.Empty;
    private bool _isActive;
    private string _accountType = string.Empty;

    public BankAccount(
        string accountNumber,
        string ownerName,
        decimal balance,
        string currency,
        string accountType)
    {
        AccountNumber = accountNumber;
        OwnerName = ownerName;
        Balance = balance;
        Currency = currency;
        AccountType = accountType;
        _isActive = true;
    }

    public string AccountNumber
    {
        get => _accountNumber;
        set => _accountNumber = RequireText(value, nameof(AccountNumber));
    }

    public string OwnerName
    {
        get => _ownerName;
        set => _ownerName = RequireText(value, nameof(OwnerName));
    }

    public decimal Balance
    {
        get => _balance;
        private set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Balance cannot be negative.");
            }

            _balance = value;
        }
    }

    public string Currency
    {
        get => _currency;
        set
        {
            string normalizedCurrency = RequireText(value, nameof(Currency)).ToUpperInvariant();
            if (normalizedCurrency.Length != 3 || !IsAsciiLetters(normalizedCurrency))
            {
                throw new ArgumentException("Currency must be a three-letter code.", nameof(value));
            }

            _currency = normalizedCurrency;
        }
    }

    public bool IsActive
    {
        get => _isActive;
        set => _isActive = value;
    }

    public string AccountType
    {
        get => _accountType;
        set => _accountType = RequireText(value, nameof(AccountType));
    }

    public void Deposit(decimal amount)
    {
        EnsureActive();
        ValidatePositiveAmount(amount);
        Balance += amount;
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

        decimal sourceBalanceAfterTransfer = Balance - amount;
        decimal recipientBalanceAfterTransfer = recipient.Balance + amount;
        Balance = sourceBalanceAfterTransfer;
        recipient.Balance = recipientBalanceAfterTransfer;
    }

    public string GetAccountSummary()
    {
        return $"{OwnerName} | {AccountType} account | {Balance:F2} {Currency} | " +
               $"{(IsActive ? "Active" : "Inactive")}";
    }

    private void EnsureActive()
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("The account is inactive.");
        }
    }

    private static void ValidatePositiveAmount(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        }
    }

    private static string RequireText(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be empty or whitespace.", parameterName);
        }

        return value.Trim();
    }

    private static bool IsAsciiLetters(string value)
    {
        foreach (char character in value)
        {
            if (character is < 'A' or > 'Z')
            {
                return false;
            }
        }

        return true;
    }
}
