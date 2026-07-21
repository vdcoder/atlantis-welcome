namespace Atlantis.Api.Citizens.Brain.Memory
{
    public class Memory
    {
        private KeyValueMemory _kvMemory;

        public Memory(uint kvCapacity) {
            _kvMemory = new KeyValueMemory(kvCapacity);
        }

        public bool TryGetValue(string key, out string? value)
        {
            return _kvMemory.TryGetValue(key, out value);
        }

        public void Add(string key, string value)
        {
            _kvMemory.Add(key, value);
        }

        public bool Remove(string key)
        {
            return _kvMemory.Remove(key);
        }

        public void Clear()
        {
            _kvMemory.Clear();
        }
    }
}
