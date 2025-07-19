using System.Globalization;

namespace Domain.ValueObjects;

public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }
    
    public Money(decimal amount, string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty", nameof(currency));

        Amount = amount;
        Currency = currency.ToUpper(); // Normalize to uppercase for consistency
    }

    // Factory method for zero value
    public static Money Zero(string currency) => new(0m, currency);

    // Addition
    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    // Subtraction
    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount - other.Amount, Currency);
    }

    // Multiplication
    public Money Multiply(decimal factor) => new(Amount * factor, Currency);

    // Division
    public Money Divide(decimal divisor)
    {
        if (divisor == 0) throw new DivideByZeroException();
        return new Money(Amount / divisor, Currency);
    }

    private void EnsureSameCurrency(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot operate on Money values with different currencies.");
    }

    public override string ToString() => $"{Amount.ToString("F2", CultureInfo.InvariantCulture)} {Currency}";

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Amount;
        yield return Currency;
    }

    // Operator Overloads
    public static Money operator +(Money a, Money b) => a.Add(b);
    public static Money operator -(Money a, Money b) => a.Subtract(b);
    public static Money operator *(Money a, decimal factor) => a.Multiply(factor);
    public static Money operator /(Money a, decimal divisor) => a.Divide(divisor);
}