namespace Atlantis.Api.Citizens.Brain.Memory
{
    public class KeyValueMemoryEntry
    { 
        private string _key;
        private string _value;
        private DateTime _timestamp;

        public KeyValueMemoryEntry(string key, string value)
        {
            _key = key;
            _value = value;
            _timestamp = DateTime.UtcNow;
        }

        public void Update(string value)
        {
            _value = value;
            _timestamp = DateTime.UtcNow;
        }

        public string Key => _key;
        public string Value => _value;
        public DateTime Timestamp => _timestamp;
    }

    public class KeyValueMemory
    {
        private readonly Dictionary<string, KeyValueMemoryEntry> _memory = new();
        private uint _capacity;

        public KeyValueMemory(uint capacity) { _capacity = capacity; }

        public bool TryGetValue(string key, out string? value)
        {
            if (_memory.TryGetValue(key, out KeyValueMemoryEntry? entry))
            {
                value = entry?.Value;
                return true;
            }
            value = null;
            return false;
        }

        public void Add(string key, string value) 
        {
            _memory[key] = new KeyValueMemoryEntry(key, value);

            while (_memory.Count > _capacity)
            {
                // remove oldest entry or throw an exception
                var oldestEntry = _memory.Values.OrderBy(e => e.Timestamp).FirstOrDefault();
                if (oldestEntry != null)
                {
                    _memory.Remove(oldestEntry.Key);
                }
            }
        }

        public bool Remove(string key)
        {
            return _memory.Remove(key);
        }

        public void Clear()
        {
            _memory.Clear();
        }
    }
}
