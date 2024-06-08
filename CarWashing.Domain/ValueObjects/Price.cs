using CSharpFunctionalExtensions;

namespace CarWashing.Domain.ValueObjects;

public class Price : ValueObject
{
    private Price(int maxValue)
    {
        MaxValue = maxValue;
    }

    public int MaxValue { get; }

    public int MinValue => MaxValue * 100;
    
    public string Format => $"{MaxValue} руб.";

    public static Price Create(int maxValue)
    {
        return new Price(maxValue);
    }
    
    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return MaxValue;
    }
}