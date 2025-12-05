using Microsoft.AspNetCore.Http;
using System.Text;

namespace Luftfartshinder.Tests
{
    // Helper class to implement ISession for testing
    // This avoids the issue with Moq not being able to mock extension methods
    public class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _store = new();

        public bool IsAvailable => true;
        public string Id => Guid.NewGuid().ToString();
        public IEnumerable<string> Keys => _store.Keys;

        public void Clear() => _store.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public void Remove(string key) => _store.Remove(key);

        public void Set(string key, byte[] value) => _store[key] = value;

#pragma warning disable CS8769 // Nullability of reference types in type of parameter doesn't match implemented member
        bool ISession.TryGetValue(string key, out byte[]? value)
        {
            if (_store.TryGetValue(key, out var bytes))
            {
                value = bytes;
                return true;
            }
            value = null;
            return false;
        }
#pragma warning restore CS8769

        // Helper methods for string operations
        public string? GetString(string key)
        {
            if (_store.TryGetValue(key, out var bytes))
            {
                return Encoding.UTF8.GetString(bytes);
            }
            return null;
        }

        public void SetString(string key, string value)
        {
            _store[key] = Encoding.UTF8.GetBytes(value);
        }
    }
}


