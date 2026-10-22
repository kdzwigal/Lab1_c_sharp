public static class Program
{
    public static void Main()
    {
        var account1 = new BankAccount("1001", "Anna Kowalska", 1_000m, "PLN", "Personal");
        var account2 = new BankAccount("1002", "Jan Nowak", 500m, "PLN", "Savings");
        var account3 = new BankAccount("1003", "Maria Wiśniewska", 250m, "PLN", "Business");

        account1.Deposit(200m);
        account2.Withdraw(50m);
        account1.TransferTo(account3, 300m);

        Console.WriteLine("Stan kont po operacjach:");
        Console.WriteLine($"{account1.AccountNumber}: {account1.GetAccountSummary()}");
        Console.WriteLine($"{account2.AccountNumber}: {account2.GetAccountSummary()}");
        Console.WriteLine($"{account3.AccountNumber}: {account3.GetAccountSummary()}");
    }
}
