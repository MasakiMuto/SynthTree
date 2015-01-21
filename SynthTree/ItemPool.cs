using System;
using System.Collections.Generic;
using System.Diagnostics;
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

		public double Threshold { get; set; }

		public ItemPool(int count)
		{
			Instance = this;
			rand = new Random();
			items = new Individual[count];
			Threshold = 100;
		}

		/// <summary>
		/// 1個体をオリジナルに変異体で構成する
		/// </summary>
		/// <param name="p"></param>
		public void Init(Tree.RootNode p)
		{
			items[0] = new Individual(p);
			UpdateInner(() => p.CloneTree().Mutate(rand), new[] { 0 });
		}

		public void MutateAll(int index)
		{
			UpdateInner(() => items[index].Tree.CloneTree().Mutate(rand), new[] { index });
		}

		void UpdateInner(Func<Tree.RootNode> childCreater, IEnumerable<int> excludeItems)
		{
			var targets = Enumerable.Range(0, items.Length).Except(excludeItems).ToArray();
			List<Individual> done = new List<Individual>(excludeItems.Select(x => items[x]));
			Individual ind;
			foreach (var j in targets)
			{
				do
				{
					ind = new Individual(childCreater());
					if (!ind.IsValidWaveform())
					{
						continue;
					}
					if (done.Any(x => !Compare(x, ind)))
					{
						continue;
					}
					break;
				} while (true);
				items[j] = ind;
				done.Add(ind);
			}
		}

		/// <summary>
		/// 十分違いがあればtrue
		/// </summary>
		/// <param name="origin"></param>
		/// <param name="another"></param>
		/// <returns></returns>
		bool Compare(Individual origin, Individual another)
		{
			var s = origin.CompareTo(another);
			return s > Threshold;
		}

		public void CrossOver(int[] index)
		{
			UpdateInner(() => {
				var p = GetPair(index.Length);
				var t = items[index[p.Item1]].Tree.CloneTree();
				t.CrossOver(items[index[p.Item2]].Tree, rand);
				return t;
			}, index);
		}

		Tuple<int, int> GetPair(int length)
		{
			Debug.Assert(length >= 2);
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
