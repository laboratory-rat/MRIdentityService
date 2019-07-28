using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Infrastructure.Entity.Enum
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EmailChangeResult
    {
        NEW,
        CANCELED,
        ACCEPTED
    }
}
