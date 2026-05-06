using System.Text.Json.Serialization;

namespace Identity_03.Models
{

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EnumUserRole
    {
        Guest = 1,
        User,
        Admin,
        SuperAdmin,
        Owner
    }
}
