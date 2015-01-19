using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AudioLib;

namespace SynthTree
{
	public static class RandomProvider
	{
		private static int seed = Environment.TickCount;

		private static ThreadLocal<Random> randomWrapper = new ThreadLocal<Random>(() =>
			new Random(Interlocked.Increment(ref seed))
		);

		public static Random GetThreadRandom()
		{
			return randomWrapper.Value;
		}
	}

	public class AutoGA
	{
		Analyzer target;
		Individual[] items;
		double[] scores;
		double[] prob;

		readonly int PoolSize = 50;
		readonly int Elite = 3;
		readonly double Mutation = 0.4;

		public int Generation { get; private set; }

		Random rand;

		public Individual BestElite { get; private set; }
		public double BestScore { get; private set; }
		public int FailCount { get; private set; }

		public Action OnUpdate;

		public AutoGA(string targetFile, int poolSize)
		{
			rand = new Random();
			this.PoolSize = poolSize;

			target = new Analyzer(targetFile);
			target.CalcSpectrogram();

			items = new Individual[poolSize];
			
			scores = new double[poolSize];
			prob = new double[poolSize];
		}

		public void Init()
		{
			for (int i = 0; i < PoolSize; i++)
			{
				items[i] = new Individual(DevelopManager.CreateInitialTree());
			}
			Generation = 0;
			FailCount = 0;
		}

		public void Run(int maxGeneration)
		{
			while (Generation < maxGeneration)
			{
				Update();
				if (OnUpdate != null)
				{
					OnUpdate();
				}
			}
		}

		void Update()
		{
			Generation++;
			Parallel.For(0, PoolSize, i => scores[i] = Eval(items[i]));
			var elite = scores.Select((x, i) => new { x, i })
				.OrderBy(a => a.x)
				.Take(Elite)
				.Select(a => a.i).ToArray();
			var best = scores[elite[0]];
			BestElite = items[elite[0]];
			BestScore = best;
			var s = scores.Sum();
			for (int i = 0; i < PoolSize; i++)
			{
				prob[i] = best / scores[i];
			}

			Individual[] newItems = new Individual[PoolSize];
			for (int i = 0; i < elite.Length; i++)
			{
				newItems[i] = items[elite[i]];
			}
			var newTrees = Enumerable.Range(elite.Length, PoolSize)
				.Select(x => CreateChildTree()).ToArray();
			Parallel.For(elite.Length, PoolSize, i => newItems[i] = new Individual(newTrees[i]));
			items = newItems;
		}

		Tree.RootNode CreateChildTree()
		{
			int p1, p2;
			p1 = RandomSelect();
			do
			{
				p2 = RandomSelect();
			} while (p2 == p1);
			var next = items[p1].Tree.CloneTree();
			if (!next.CrossOver(items[p2].Tree, rand))
			{
				FailCount++;
			}
			if (rand.NextDouble() < Mutation)
			{
				next.Mutate(rand);
			}
			return next;
		}

		int RandomSelect()
		{
			var t = rand.NextDouble() * prob.Sum();
			var s = 0.0;
			for (int i = 0; i < PoolSize; i++)
			{
				s += prob[i];
				if (s > t)
				{
					return i;
				}
			}
			return PoolSize - 1;
		}


		double Eval(Individual item)
		{
			return item.CompareTo(target);
		}

	}
}
