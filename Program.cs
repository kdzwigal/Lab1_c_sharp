using System.Text.Json;

public static class Program
{
    public static void Main()
    {
        var customer1 = new Customer("C001", "Anna", "Kowalska");
        var customer2 = new Customer("C002", "Jan", "Nowak");
        var customer3 = new Customer("C003", "Maria", "Wiśniewska");
        var customer4 = new Customer("C004", "Piotr", "Zieliński");
        var customer5 = new Customer("C005", "Ewa", "Wójcik");
        var customer6 = new Customer("C006", "Tomasz", "Kamiński");

        var account1 = new BankAccount("1001", customer1, 0m, "PLN", "Personal");
        var account2 = new BankAccount("1002", customer2, 0m, "PLN", "Savings");
        var account3 = new BankAccount("1003", customer3, 0m, "PLN", "Business");
        var account4 = new BankAccount("1004", customer4, 0m, "PLN", "Personal");
        var account5 = new BankAccount("1005", customer5, 0m, "PLN", "Savings");
        var account6 = new BankAccount("1006", customer6, 0m, "PLN", "Business");

        account1.Deposit(1_000m);
        account2.Deposit(2_500m);
        account1.Withdraw(200m);
        account2.TransferTo(account3, 500m);
        account3.Withdraw(100m);

        account4.Deposit(4_000m);
        account4.TransferTo(account5, 750m);
        account5.Deposit(1_200m);
        account5.Withdraw(300m);
        account6.Deposit(8_500m);
        account6.TransferTo(account4, 1_000m);

        List<BankAccount> accounts = new()
        {
            account1, account2, account3, account4, account5, account6
        };

        Console.WriteLine("Stan wszystkich kont po operacjach:");
        foreach (BankAccount account in accounts)
        {
            Console.WriteLine(account.GetAccountSummary());
            Console.WriteLine($"Aktualne saldo: {account.GetCurrentBalance()}");
        }

        Console.WriteLine();
        Console.WriteLine($"Historia transakcji konta {account2.AccountNumber}:");
        account2.DisplayTransactionHistory();

        JsonSerializerOptions jsonOptions = new()
        {
            WriteIndented = true
        };
        string json = JsonSerializer.Serialize(accounts, jsonOptions);
        File.WriteAllText("accounts.json", json);

        Console.WriteLine();
        Console.WriteLine("Dane kont zapisano do pliku accounts.json.");
    }
}
