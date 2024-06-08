using CSharpFunctionalExtensions;

namespace CarWashing.Persistence.Entities.ValueObjects;

public class TimeEntity : ValueObject
{
    private int _seconds;
    private string _format;
    
    public TimeEntity() {}

    public TimeEntity(int minutes)
    {
        Minutes = minutes;
        _format = string.Empty;
    }
    public int Minutes { get; set; }

    public int Seconds
    {
        get => Minutes * 60;
        set => _seconds = value;
    }

    public string Format
    {
        get => $"{Minutes} мин.";
        set => _format = value;
    }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Minutes;
    }
}