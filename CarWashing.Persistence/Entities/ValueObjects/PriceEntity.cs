using CSharpFunctionalExtensions;

namespace CarWashing.Persistence.Entities.ValueObjects;

public class PriceEntity : ValueObject
{
    private int _minValue;
    private string _format;
    
    public PriceEntity() {}

    public PriceEntity(int maxValue)
    {
        MaxValue = maxValue;
        _format = string.Empty;
    }
    public int MaxValue { get; set; }

    public int MinValue
    {
        get => MaxValue * 100;
        set => _minValue = value;
    }

    public string Format
    {
        get => $"{MaxValue} руб.";
        set => _format = value;
    } 

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return MaxValue;
    }
}