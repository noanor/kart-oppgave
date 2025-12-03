using System.Text.Json;

namespace Luftfartshinder.Extensions
{
    /// <summary>
    /// Extension methods for saving and loading strongly-typed objects (string, int, bool, etc...) in the session using JSON serialization.
    /// They unify JSON settings and make it easy to save complex objects in <see cref="ISession"/>.
    /// </summary>
    public static class SessionExtensions
    {
        /// <summary>
        /// Shared JsonSerializerOptions used for all session serialization/deserialization (converting objects to and from JSON).
        /// Uses the Web defaults to produce JSON suitable for browser/server.
        /// </summary>
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        /// <summary>
        /// Serialize (converting) <paramref name="value"/> to JSON and store it under <paramref name="key"/> in the session.
        /// </summary>
        /// <typeparam name="T">Type of the value to store.</typeparam>
        /// <param name="session">The session instance (this).</param>
        /// <param name="key">The session key to use.</param>
        /// <param name="value">The value to serialize and store.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="session"/> or <paramref name="key"/> is null.</exception>
        public static void Set<T>(this ISession session, string key, T value)
        {
            if (session is null) throw new ArgumentNullException(nameof(session));
            if (key is null) throw new ArgumentNullException(nameof(key));

            var json = JsonSerializer.Serialize(value, JsonOptions);
            session.SetString(key, json);
        }

        /// <summary>
        /// Retrieve a typed value from session by <paramref name="key"/>, deserializing the stored JSON.
        /// Returns <c>default(T)</c> if the key is not present.
        /// </summary>
        /// <typeparam name="T">Requested return type.</typeparam>
        /// <param name="session">The session instance (this).</param>
        /// <param name="key">The session key to read.</param>
        /// <returns>The deserialized value, or <c>default(T)</c> when the key does not exist or the stored value is null.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="session"/> or <paramref name="key"/> is null.</exception>
        public static T? Get<T>(this ISession session, string key)
        {
            if (session is null) throw new ArgumentNullException(nameof(session));
            if (key is null) throw new ArgumentNullException(nameof(key));

            var json = session.GetString(key);
            return json is null ? default : JsonSerializer.Deserialize<T>(json, JsonOptions);
        }
    }
}