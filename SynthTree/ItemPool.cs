using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree
{
	public class ItemPool
	{
	
		public static ItemPool Instance { get; private set; }
		Individual[] items;
		Random rand;
		public int Generation { get; private set; }

		public Individual this[int i]{get{return items[i];}}

		public ItemPool(int count)
		{
			Instance = this;
			rand = new Random();
			items = new Individual[count];
		}

		/// <summary>
		/// 1個体をオリジナルに変異体で構成する
		/// </summary>
		/// <param name="p"></param>
		public void Init(Tree.RootNode p, int index)
		{
			items[index] = new Individual(p);
			for (int i = 0; i < items.Length; i++)
			{
				if (index == i)
				{
					continue;
				}
				var t = p.CloneTree();
				t.Mutate(rand);
				items[i] = new Individual(t);
				if (!Check(items[index], items[i]))
				{
					i--;
					continue;
				}
			}
		}

		bool Check(Individual origin, Individual another)
		{
			var s = origin.CompareTo(another);
			return s > 1;
		}

		public void CrossOver(int[] index)
		{
			var parent = items.Where((x, i) => index.Contains(i)).Select(x=>x.Tree).ToArray();
			for (int i = 0; i < items.Length; i++)
			{
				if (!index.Contains(i))
				{
					var p = GetPair(index.Length);
					var t = parent[p.Item1];
					if (!t.CrossOver(parent[p.Item2], rand))
					{
						System.Diagnostics.Debugger.Break();
					}

					items[i] = new Individual(t);
				}
			}
			Generation++;
		}

		Tuple<int, int> GetPair(int length)
		{
			int p1 = rand.Next(length);
			int p2;
			do
			{
				p2 = rand.Next(length);
			} while (p1 == p2);
			return new Tuple<int, int>(p1, p2);
		}
	}
}
