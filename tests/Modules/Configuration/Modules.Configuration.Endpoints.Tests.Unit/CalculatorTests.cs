namespace Modules.Configuration.Endpoints.Tests.Unit;

public class Calculator
{
    public int Add(int a, int b) => a + b;
    public bool IsEven(int number) => number % 2 == 0;
}

public class CalculatorTests
{
    [Fact]
    public void Add_ShouldReturn_SumOfTwoNumbers()
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var result = calculator.Add(3, 5);

        // Assert
        result.Should().Be(8);
    }

    [Theory]
    [InlineData(2, true)]
    [InlineData(3, false)]
    public void IsEven_ShouldReturn_TrueForEvenNumbers(int number, bool expected)
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var isEven = calculator.IsEven(number);

        // Assert
        isEven.Should().Be(expected);
    }
}