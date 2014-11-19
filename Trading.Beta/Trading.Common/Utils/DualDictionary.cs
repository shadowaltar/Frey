using System;
using System.Collections.Generic;
using System.Data;

namespace Trading.Common.Utils
{
    public class DualDictionary<TK1, TK2, TV>
    {
        private Dictionary<TK1, Dictionary<TK2, TV>> primaryKeyDictionary = new Dictionary<TK1, Dictionary<TK2, TV>>();
        private Dictionary<TK2, Dictionary<TK1, TV>> secondaryKeyDictionary = new Dictionary<TK2, Dictionary<TK1, TV>>();

        public DualDictionary()
        {
            if (typeof(TK1) == typeof(TK2))
                throw new ArgumentException("Cannot use keys of the same type");
        }

        public void Add(TK1 key1, TK2 key2, TV value)
        {
            if (!primaryKeyDictionary.ContainsKey(key1))
            {
                primaryKeyDictionary[key1] = new Dictionary<TK2, TV>();
            }
            if (!secondaryKeyDictionary.ContainsKey(key2))
            {
                secondaryKeyDictionary[key2] = new Dictionary<TK1, TV>();
            }

            primaryKeyDictionary[key1][key2] = value;
            secondaryKeyDictionary[key2][key1] = value;
        }

        public void AddValues(TK1 key1, Dictionary<TK2, TV> dictionary)
        {
            if (!primaryKeyDictionary.ContainsKey(key1))
            {
                primaryKeyDictionary[key1] = new Dictionary<TK2, TV>();
            }
            var d = primaryKeyDictionary[key1];
            foreach (var dic in dictionary)
            {
                var key2 = dic.Key;
                d[key2] = dic.Value;

                if (!secondaryKeyDictionary.ContainsKey(key2))
                {
                    secondaryKeyDictionary[key2] = new Dictionary<TK1, TV>();
                }
                secondaryKeyDictionary[key2][key1] = dic.Value;
            }
        }

        public void AddValues(TK2 key2, Dictionary<TK1, TV> dictionary)
        {
            if (!secondaryKeyDictionary.ContainsKey(key2))
            {
                secondaryKeyDictionary[key2] = new Dictionary<TK1, TV>();
            }
            var d = secondaryKeyDictionary[key2];
            foreach (var dic in dictionary)
            {
                var key1 = dic.Key;
                d[key1] = dic.Value;

                if (!primaryKeyDictionary.ContainsKey(key1))
                {
                    primaryKeyDictionary[key1] = new Dictionary<TK2, TV>();
                }
                primaryKeyDictionary[key1][key2] = dic.Value;
            }
        }

        public bool ContainsKey(TK1 key1)
        {
            return primaryKeyDictionary.ContainsKey(key1);
        }

        public bool ContainsKey(TK2 key2)
        {
            return secondaryKeyDictionary.ContainsKey(key2);
        }

        public bool TryGet(TK1 key1, out Dictionary<TK2, TV> values)
        {
            if (ContainsKey(key1))
            {
                values = primaryKeyDictionary[key1];
                return true;
            }
            values = null;
            return false;
        }

        public bool TryGet(TK2 key2, out Dictionary<TK1, TV> values)
        {
            if (ContainsKey(key2))
            {
                values = secondaryKeyDictionary[key2];
                return true;
            }
            values = null;
            return false;
        }

        public void Remove(TK1 key1)
        {
            if (!primaryKeyDictionary.ContainsKey(key1))
                return;

            primaryKeyDictionary[key1].Clear();
            foreach (var pair in secondaryKeyDictionary)
            {
                pair.Value.Remove(key1);
            }
        }

        public void Remove(TK1 key1, TK2 key2, TV value)
        {
            if (!primaryKeyDictionary.ContainsKey(key1))
                return;
            if (!secondaryKeyDictionary.ContainsKey(key2))
                return;

            primaryKeyDictionary[key1].Remove(key2);
            secondaryKeyDictionary[key2].Remove(key1);
        }

        public TV this[TK1 key1, TK2 key2]
        {
            get { return primaryKeyDictionary[key1][key2]; }
            set
            {
                primaryKeyDictionary[key1][key2] = value;
                secondaryKeyDictionary[key2][key1] = value;
            }
        }

        public Dictionary<TK2, TV> this[TK1 key]
        {
            get { return primaryKeyDictionary[key]; }
        }

        public Dictionary<TK1, TV> this[TK2 key]
        {
            get { return secondaryKeyDictionary[key]; }
        }

        public static DualDictionary<TK1, TK2, TV> From<TSource>(IEnumerable<TSource> sources,
            Func<TSource, TK1> key1Converter,
            Func<TSource, TK2> key2Converter,
            Func<TSource, TV> valueConverter)
        {
            var result = new DualDictionary<TK1, TK2, TV>();
            foreach (var entry in sources)
            {
                TV value = valueConverter(entry);
                TK1 key1 = key1Converter(entry);
                TK2 key2 = key2Converter(entry);
                result.Add(key1, key2, value);
            }
            return result;
        }
    }
}