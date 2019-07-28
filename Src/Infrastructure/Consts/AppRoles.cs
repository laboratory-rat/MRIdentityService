using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Infrastructure.Consts
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AppRoles
    {
        USER,
        PROVIDER_MANAGER,
        MANAGER,
        ADMINISTRATOR
    }
}
