using System.Text.Json.Serialization;

namespace CarWashing.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Role
{
    Administrator = 1,
    Employee = 2,
    User = 3
}