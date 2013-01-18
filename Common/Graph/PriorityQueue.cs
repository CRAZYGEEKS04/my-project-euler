using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEuler.Common.Graph
{
    public class PriorityQueue<TKey, TValue>
    {
        private IEqualityComparer<TKey> kHelper;
        private IComparer<TValue> vHelper;
        private List<KeyValuePair<TKey, TValue>> data;
        private Dictionary<TKey, int> index;

        public int Count
        {
            get { return data.Count - 1; }
        }

        public KeyValuePair<TKey, TValue> Min
        {
            get
            {
                if (data.Count == 1)
                    throw new InvalidOperationException("queue is empty");
                return data[1];
            }
        }

        private void Exchange(int x, int y)
        {
            KeyValuePair<TKey, TValue> tmp = data[x];

            data[x] = data[y];
            data[y] = tmp;
            index[data[x].Key] = x;
            index[data[y].Key] = y;
        }

        private void MinHeapify(int i)
        {
            int l = i * 2;
            int r = i * 2 + 1;
            int min = i;

            if (l < data.Count && vHelper.Compare(data[l].Value, data[i].Value) < 0)
                min = l;
            if (r < data.Count && vHelper.Compare(data[r].Value, data[min].Value) < 0)
                min = r;

            if (min != i)
            {
                Exchange(i, min);
                MinHeapify(min);
            }
        }

        public PriorityQueue(IEqualityComparer<TKey> kh, IComparer<TValue> vh)
        {
            kHelper = kh;
            vHelper = vh;
            data = new List<KeyValuePair<TKey, TValue>>();
            index = new Dictionary<TKey, int>(kHelper);
            data.Add(new KeyValuePair<TKey, TValue>());
        }

        public KeyValuePair<TKey, TValue> ExtractMin()
        {
            KeyValuePair<TKey, TValue> ret = Min;

            data[1] = data[data.Count - 1];
            index[data[1].Key] = 1;
            data.RemoveAt(data.Count - 1);
            index.Remove(ret.Key);
            MinHeapify(1);

            return ret;
        }

        public void DecreaseKey(TKey key, TValue value)
        {
            int id = index[key];

            if (vHelper.Compare(data[id].Value, value) < 0)
                throw new InvalidOperationException("new value is larger than current value");
            data[id] = new KeyValuePair<TKey, TValue>(key, value);

            while (id > 1 && vHelper.Compare(data[id].Value, data[id / 2].Value) < 0)
            {
                Exchange(id, id / 2);
                id /= 2;
            }
        }

        public void Add(TKey key, TValue value)
        {
            if (index.ContainsKey(key))
                throw new InvalidOperationException("key already exists");
            index.Add(key, data.Count);
            data.Add(new KeyValuePair<TKey, TValue>(key, value));
            DecreaseKey(key, value);
        }
    }
}