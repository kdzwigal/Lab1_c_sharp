public class Customer
{
    public Customer(string customerId, string firstName, string lastName)
    {
        CustomerId = RequireText(customerId, nameof(customerId));
        FirstName = RequireText(firstName, nameof(firstName));
        LastName = RequireText(lastName, nameof(lastName));
    }

    public string CustomerId { get; }

    public string FirstName { get; }

    public string LastName { get; }

    public string FullName => $"{FirstName} {LastName}";

    private static string RequireText(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be empty or whitespace.", parameterName);
        }

        return value.Trim();
    }
}
