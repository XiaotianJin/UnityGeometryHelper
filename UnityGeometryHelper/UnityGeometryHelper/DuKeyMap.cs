using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityGeometryHelper
{
    public class Tuple<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;
        public Tuple()
        {

        }

        public Tuple(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }
    }

    public class DupKeyMap<TKey, TValue>
    {
        public List<Tuple<TKey, TValue>> StorageList =
            new List<Tuple<TKey, TValue>>();

        public TValue GetFirst(TKey key)
        {
            for (int i = 0; i < StorageList.Count; i++)
            {
                if (StorageList[i].Key.Equals(key))
                {
                    return StorageList[i].Value;
                }
            }

            throw new System.Exception(string.Format("{0} Not found in DupkeyMap",
                key.ToString()));
        }

        public List<TValue> GetAll(TKey key)
        {
            List<TValue> res = new List<TValue>();

            for (int i = 0; i < StorageList.Count; i++)
            {
                if (StorageList[i].Key.Equals(key))
                {
                    res.Add(StorageList[i].Value);
                }
            }

            return res;
        }

        public bool ContainKey(TKey key)
        {
            for (int i = 0; i < StorageList.Count; i++)
            {
                if (StorageList[i].Key.Equals(key))
                {
                    return true;
                }
            }

            return false;
        }

        public void Add(TKey key, TValue value)
        {
            StorageList.Add(new Tuple<TKey, TValue>(key, value));
        }

        public void RemoveFirst(TKey key)
        {
            for (int i = 0; i < StorageList.Count; i++)
            {
                if (StorageList[i].Key.Equals(key))
                {
                    StorageList.RemoveAt(i);
                    return;
                }
            }
        }

        public void RemoveAll(TKey key)
        {
            for (int i = 0; i < StorageList.Count; i++)
            {
                if (StorageList[i].Key.Equals(key))
                {
                    StorageList.RemoveAt(i);
                    i--;
                }
            }
        }

        public void ChangeFirstKeyTo(TKey oldKey, TKey newKey)
        {
            for (int i = 0; i < StorageList.Count; i++)
            {
                if (StorageList[i].Key.Equals(oldKey))
                {
                    StorageList[i].Key = newKey;
                    return;
                }
            }
        }

        public void ChangeAllKeyTo(TKey oldKey, TKey newKey)
        {
            for (int i = 0; i < StorageList.Count; i++)
            {
                if (StorageList[i].Key.Equals(oldKey))
                {
                    StorageList[i].Key = newKey;
                }
            }
        }

        // a>b:true
        // 就会从小到大排列
        public delegate bool ISBIGGER(TKey a, TKey b);
        public void SortedByKey(ISBIGGER isBigger)
        {
            for (int i = 0; i < StorageList.Count; i++)
            {
                for (int j = i + 1; j < StorageList.Count; j++)
                {
                    if (isBigger(StorageList[i].Key, StorageList[j].Key))
                    {
                        Tuple<TKey, TValue> swap = StorageList[i];
                        StorageList[i] = StorageList[j];
                        StorageList[j] = swap;
                    }
                }
            }
        }

        public List<TKey> FindByValues(TValue value)
        {
            List<TKey> keys = new List<TKey>();

            for (int i = 0; i < StorageList.Count; i++)
            {
                if (StorageList[i].Value.Equals(value))
                {
                    keys.Add(StorageList[i].Key);
                }
            }

            return keys;
        }

        public List<TKey> AllKey
        {
            get
            {
                List<TKey> keys = new List<TKey>();

                for (int i = 0; i < StorageList.Count; i++)
                {
                    keys.Add(StorageList[i].Key);
                }

                return keys;
            }
        }

        public List<TValue> AllValue
        {
            get
            {
                List<TValue> values = new List<TValue>();

                for (int i = 0; i < StorageList.Count; i++)
                {
                    values.Add(StorageList[i].Value);
                }

                return values;
            }
        }

        public List<TKey> GetNonDupKeys()
        {
            List<TKey> keys = AllKey;
            List<TKey> res = new List<TKey>();
            for (int i = 0; i < keys.Count; i++)
            {
                if (!res.Contains(keys[i]))
                {
                    res.Add(keys[i]);
                }
            }

            return res;
        }

        public TValue this[TKey key]
        {
            get { return GetFirst(key); }
            set
            {
                if (this.ContainKey(key))
                {
                    for (int i = 0; i < StorageList.Count; i++)
                    {
                        if (StorageList[i].Key.Equals(key))
                        {
                            StorageList[i].Value = value;
                            return;
                        }
                    }
                }
                else
                {
                    this.Add(key, value);
                }
            }
        }

        public int Count
        {
            get { return StorageList.Count; }
        }
    }
}
