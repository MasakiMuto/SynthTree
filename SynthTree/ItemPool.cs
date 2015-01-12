using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree
{
	public class ItemPool
	{
		public class ItemSet
		{
			public readonly Tree.RootNode Tree;
			public readonly Unit.Renderer Topology;
			public readonly string Sound;
			public readonly AudioLib.Analyzer Analyzer;
			public readonly float[] Data;

			public ItemSet(Tree.RootNode tree)
			{
				Tree = tree;
				Topology = DevelopManager.DevelopTopology(tree);
				Sound = System.IO.Path.GetTempFileName();
				Data = Enumerable.Range(0, DevelopManager.TableLength).Select(x => (float)Topology.RequireValue(x)).ToArray();
				Normalize();
				Analyzer = new AudioLib.Analyzer(Data, (int)FileUtil.SampleRate);
				Analyzer.CalcSpectrogram();
				using (var f = new FileUtil(Sound))
				{
					f.Write(Data);
				}
			}

			void Normalize()
			{
				float absMax = Data.Select(x => Math.Abs(x)).Max();
				for (int i = 0; i < Data.Length; i++)
				{
					Data[i] /= absMax;
				}
			}

		}

		public static ItemPool Instance { get; private set; }
		ItemSet[] items;
		Random rand;
		public int Generation { get; private set; }

		public ItemSet this[int i]{get{return items[i];}}

		public ItemPool()
		{
			Instance = this;
			rand = new Random();
			items = new ItemSet[10];
		}

		/// <summary>
		/// 1個体をオリジナルに変異体で構成する
		/// </summary>
		/// <param name="p"></param>
		public void Init(Tree.RootNode p, int index)
		{
			items[index] = new ItemSet(p);
			for (int i = 0; i < items.Length; i++)
			{
				if (index == i)
				{
					continue;
				}
				var t = p.CloneTree();
				t.Mutate(rand);
				items[i] = new ItemSet(t);
			}
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

					items[i] = new ItemSet(t);
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
