using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Infrastructure.Enum
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ProviderUserRole
    {
        USE,
        ANALYTICS,
        EDIT,
        ADMINISTRATOR,
        OWNER
    }
}
