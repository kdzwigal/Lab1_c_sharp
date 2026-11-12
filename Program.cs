using System.Text.Json;

public static class Program
{
    public static void Main()
    {
        Console.WriteLine(BankHelper.GetBankInformation());
        Console.WriteLine($"Przykładowy numer konta: {BankHelper.GenerateSampleAccountNumber(1005)}");

        var savings = new SavingsAccount(
            "1001",
            new Customer("C001", "Anna", "Kowalska"),
            0m,
            "PLN",
            annualInterestRate: 0.06m);
        var business = new BusinessAccount(
            "1002",
            new Customer("C002", "Jan", "Nowak"),
            0m,
            "PLN",
            taxId: "5250001001",
            transferCommissionRate: 0.01m);
        var personal = new PersonalAccount(
            "1003",
            new Customer("C003", "Maria", "Wiśniewska"),
            0m,
            "PLN",
            freeTransfersPerMonth: 1);
        var secondSavings = new SavingsAccount(
            "1004",
            new Customer("C004", "Piotr", "Zieliński"),
            0m,
            "PLN",
            annualInterestRate: 0.04m);

        List<BankAccount> accounts = new() { savings, business, personal, secondSavings };

        savings.Deposit(3_000m);
        savings.ApplyMonthlyInterest();
        savings.TransferTo(business, 500m);

        business.Deposit(8_000m);
        business.TransferTo(personal, 1_000m);
        business.ApplyMonthlyFee();

        personal.Deposit(1_500m);
        personal.TransferTo(secondSavings, 200m);
        personal.TransferTo(secondSavings, 100m);
        personal.Withdraw(50m);

        secondSavings.Deposit(2_000m);
        secondSavings.ApplyMonthlyInterest();
        secondSavings.ApplyMonthlyFee();

        Console.WriteLine();
        Console.WriteLine("Stan wszystkich kont po operacjach:");
        foreach (BankAccount account in accounts)
        {
            Console.WriteLine(account.GetAccountSummary());
            Console.WriteLine($"Opłata miesięczna: {account.CalculateMonthlyFee():F2} {account.Currency}");
        }

        Console.WriteLine();
        Console.WriteLine($"Historia transakcji konta firmowego {business.AccountNumber}:");
        business.DisplayTransactionHistory();

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
