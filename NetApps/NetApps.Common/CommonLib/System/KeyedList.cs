using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace System.Data.Common
{
    public class KeyedList<TValue> :
        ICollection<KeyValuePair<String, TValue>>,
        IEnumerable<KeyValuePair<String, TValue>>,
        IEnumerable,
        IDictionary<String, TValue>,
        IReadOnlyCollection<KeyValuePair<String, TValue>>,
        IReadOnlyDictionary<String, TValue>,
        ICollection,
        IDictionary,
        IDeserializationCallback,
        ISerializable
    //where String : notnull
    {
        private readonly Dictionary<string, object> bag = new Dictionary<string, object>();

        public TValue this[string key] => throw new NotImplementedException();

        public object? this[object key]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        TValue IDictionary<string, TValue>.this[string key]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public IEnumerable<string> Keys => throw new NotImplementedException();

        public IEnumerable<TValue> Values => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public bool IsFixedSize => throw new NotImplementedException();

        ICollection<string> IDictionary<string, TValue>.Keys => throw new NotImplementedException();

        ICollection IDictionary.Keys => throw new NotImplementedException();

        ICollection<TValue> IDictionary<string, TValue>.Values => throw new NotImplementedException();

        ICollection IDictionary.Values => throw new NotImplementedException();

        public void Add(string key, TValue value)
        {
            throw new NotImplementedException();
        }

        public void Add(KeyValuePair<string, TValue> item)
        {
            throw new NotImplementedException();
        }

        public void Add(object key, object? value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<string, TValue> item)
        {
            throw new NotImplementedException();
        }

        public bool Contains(object key)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(string key)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<string, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<String, TValue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public void OnDeserialization(object? sender)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, TValue> item)
        {
            throw new NotImplementedException();
        }

        public void Remove(object key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out TValue value)
        {
            throw new NotImplementedException();
        }

        Collections.IEnumerator Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}