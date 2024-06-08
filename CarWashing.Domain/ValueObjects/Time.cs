using CSharpFunctionalExtensions;

namespace CarWashing.Domain.ValueObjects;

public class Time : ValueObject
{
    private Time(int minutes)
    {
        Minutes = minutes;
    }

    public int Minutes { get; }

    public int Seconds => Minutes * 60;

    public string Format => $"{Minutes} мин.";
    
    public static Time Create(int minutes)
    {
        return new Time(minutes);
    }
    
    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Minutes;
    }
}