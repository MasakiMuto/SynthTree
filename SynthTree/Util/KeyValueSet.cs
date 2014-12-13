using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree.Util
{
	public class KeyValueSet<TKey, TValue>  : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		List<KeyValuePair<TKey, TValue>> items;

		public KeyValueSet()
		{
			items = new List<KeyValuePair<TKey, TValue>>();
		}

		public void Add(TKey key, TValue value)
		{
			items.Add(new KeyValuePair<TKey, TValue>(key, value));
		}



		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return items.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return items.GetEnumerator();
		}
	}
}
