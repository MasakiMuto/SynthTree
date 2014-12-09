using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree
{
	class ItemPool
	{
		public static ItemPool Instance { get; private set; }
		SynthTree.Tree.RootNode[] items;
		Random rand;
		public int Generation { get; private set; }

		public ItemPool()
		{
			Instance = this;
			rand = new Random();
			items = new Tree.RootNode[10];
		}

		/// <summary>
		/// 1個体をオリジナルに変異体で構成する
		/// </summary>
		/// <param name="p"></param>
		public void Init(Tree.RootNode p)
		{
			for (int i = 0; i < items.Length; i++)
			{
				items[i] = p.Mutate();
				//items[i] = SynthParam.Mutate(p);
			}
		}

		public void CrossOver(int[] index)
		{
			var parent = items.Where((x, i) => index.Contains(i)).ToArray();
			for (int i = 0; i < items.Length; i++)
			{
				if (!index.Contains(i))
				{
					var p = GetPair(index.Length);
					items[i] = parent[p.Item1].CrossOver(parent[p.Item2]);
					//items[i] = parent[p.Item1].CrossOver(rand, parent[p.Item2]);
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
