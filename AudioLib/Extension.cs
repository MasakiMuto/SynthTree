using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioLib
{
	public static class Extension
	{
		public static void ForAll<T>(this IEnumerable<T> collection, Action<T> act)
		{
			foreach (var item in collection)
			{
				act(item);
			}
		}
	}
}
