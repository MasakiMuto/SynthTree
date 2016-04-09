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

		public double ThresholdMin { get; set; }
		public double ThresholdMax { get; set; }

		Stack<Individual[]> history, undoHistory;

		public ItemPool(int count)
		{
			Instance = this;
			rand = new Random();
			items = new Individual[count];
			ThresholdMin = 0;
			ThresholdMax = double.MaxValue;
			history = new Stack<Individual[]>();
			undoHistory = new Stack<Individual[]>();
		}

		public void Undo()
		{
			if (history.Count == 0)
			{
				return;
			}
			Generation--;
			undoHistory.Push(items.Clone() as Individual[]);
			items = history.Pop();
			
		}

		public void Redo()
		{
			if (undoHistory.Count == 0)
			{
				return;
			}
			Generation++;
			history.Push(items.Clone() as Individual[]);
			items = undoHistory.Pop();
		}

		/// <summary>
		/// 1個体をオリジナルに変異体で構成する
		/// </summary>
		/// <param name="p"></param>
		public void Init(Tree.RootNode p)
		{
			items[0] = new Individual(p, true);
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
			int i = 0;
			history.Push(items.Clone() as Individual[]);
			undoHistory.Clear();
			foreach (var j in targets)
			{
				i = 0;
				do
				{
					i++;
					ind = new Individual(childCreater(), true);
					if (!ind.IsValidWaveform())
					{
						continue;
					}
					if (done.Any(x => !Compare(x, ind)))
					{
						continue;
					}
					break;
				} while (i < 100);
				items[j] = ind;
				done.Add(ind);
			}
			Generation++;
		}

		/// <summary>
		/// 十分違いがあればtrue
		/// </summary>
		/// <param name="origin"></param>
		/// <param name="another"></param>
		/// <returns></returns>
		bool Compare(Individual origin, Individual another)
		{
			return true;
			var s = origin.CompareTo(another);
			return s > ThresholdMin && s < ThresholdMax;
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
