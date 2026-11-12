public enum TransactionType
{
    Deposit,
    Withdrawal,
    TransferOut,
    TransferIn,
    Fee,
    Interest
}

public class Transaction
{
    public Transaction(
        TransactionType type,
        decimal amount,
        string currency,
        string description,
        string? sourceAccountNumber = null,
        string? destinationAccountNumber = null)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        }

        Type = type;
        Amount = amount;
        Currency = currency;
        Description = description;
        SourceAccountNumber = sourceAccountNumber;
        DestinationAccountNumber = destinationAccountNumber;
        OccurredAt = DateTime.Now;
    }

    public TransactionType Type { get; }

    public decimal Amount { get; }

    public string Currency { get; }

    public string Description { get; }

    public DateTime OccurredAt { get; }

    public string? SourceAccountNumber { get; }

    public string? DestinationAccountNumber { get; }
}
