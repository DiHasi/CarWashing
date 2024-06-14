using System.Text.Json.Serialization;

namespace CarWashing.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Status
{
    InProgress = 1,
    Completed = 2
}