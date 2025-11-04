using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Luftfartshinder.Extensions
{
    public static class SessionExtensions
    {
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public static void Set<T>(this ISession session, string key, T value) =>
            session.SetString(key, JsonSerializer.Serialize(value, _json));

        public static T? Get<T>(this ISession session, string key)
        {
            var s = session.GetString(key);
            return s is null ? default : JsonSerializer.Deserialize<T>(s, _json);
        }
    }
}
