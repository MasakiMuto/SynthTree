using System;
using System.Collections.Generic;
using System.Diagnostics;
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

	[Serializable]
	public class GAResult
	{
		public Tree.RootNode Tree;
		public string TargetFileName;

		public const string Extension = ".result";

		public static bool IsGAResultFile(string fileName)
		{
			return System.IO.Path.GetExtension(fileName) == Extension;
		}

		public static void Save(string fileName, Tree.RootNode tree, string target)
		{
			var res = new GAResult();
			res.Tree = tree;
			res.TargetFileName = target;
			var ser = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			using (var file = System.IO.File.OpenWrite(fileName))
			{
				ser.Serialize(file, res);
			}
		}

		public static GAResult Load(string fileName)
		{
			var ser = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			using (var file = System.IO.File.OpenRead(fileName))
			{
				return ser.Deserialize(file) as GAResult;
			}
		}

	}

	public class AutoGA
	{
		Analyzer target;
		Individual[] items;
		public double[] Scores { get; private set; }
		double[] prob;

		readonly int PoolSize;
		readonly int Elite = 3;
		readonly double Mutation = 0.1;
		readonly double MaxMutation = 0.5;

		public int Generation { get; private set; }


		public Individual BestElite { get; private set; }
		public double BestScore { get; private set; }
		public int FailCount { get; private set; }

		public bool IsRunning { get; private set; }

		public Action OnUpdate;

		double mutationRate;
		int continueCount;

		public Tree.RootNode Initial { get; set; }

		public System.Threading.CancellationToken Cancell;

		readonly string fileName;

		public AutoGA(string targetFile, int poolSize)
		{
			fileName = targetFile;
			this.PoolSize = poolSize;
			Elite = 1;
			target = new Analyzer(targetFile);
			Settings.Instance.SamplingFreq = target.SampleRate;
			target.Normalize();
			target.CalcSpectrogram();
			

			items = new Individual[poolSize];
			
			Scores = new double[poolSize];
			prob = new double[poolSize];
		}

		public void Init()
		{
			var trees = Enumerable.Range(0, Initial == null ? PoolSize : PoolSize - 1).Select(x => DevelopManager.CreateInitialTree()).ToArray();
			if (Initial != null)
			{
				items[PoolSize - 1] = new Individual(Initial);
			}
			Parallel.For(0, Initial == null ? PoolSize : PoolSize - 1, i => items[i] = new Individual(new ParameterOptimizer(trees[i], target).Run()));
			
			Generation = 0;
			FailCount = 0;
			continueCount = 0;
			BestScore = double.PositiveInfinity;
		}

		public void Run(int maxGeneration)
		{
			if (IsRunning)
			{
				return;
			}
			IsRunning = true;
			while (Generation < maxGeneration && (Cancell == null || !Cancell.IsCancellationRequested))
			{
				Update();
				if (OnUpdate != null)
				{
					OnUpdate();
				}
			}
			IsRunning = false;
		}

		public void SaveResult(string fileName)
		{
			GAResult.Save(fileName, BestElite.Tree, this.fileName);
		}

		void Update()
		{
			Generation++;
			Parallel.For(0, PoolSize, i => Scores[i] = Eval(items[i]));
			var elite = Scores.Select((x, i) => new { x, i })
				.OrderBy(a => a.x)
				.Take(Elite)
				.Select(a => a.i).ToArray();
			var best = Scores[elite[0]];
			var lastElite = BestScore;
			BestElite = items[elite[0]];
			BestScore = best;
			if (lastElite == BestScore)
			{
				continueCount++;
				continueCount = Math.Min(continueCount, 5);
				mutationRate = Mutation + (MaxMutation - Mutation) * continueCount / 5.0;
			}
			else
			{
				continueCount = 0;
				mutationRate = Mutation;
			}
			
			mutationRate = (BestScore / lastElite) * Mutation;
			
			CreateSelectionTable();

			Individual[] newItems = new Individual[PoolSize];
			for (int i = 0; i < elite.Length; i++)
			{
				newItems[i] = items[elite[i]];
			}
			Parallel.For(elite.Length, PoolSize, i => newItems[i] = new Individual(CreateChildTree()));
			items = newItems;
		}

		void CreateSelectionTable()
		{
			var order = Scores.Select((x, i) => new { x, i })
				.OrderBy(a => a.x)
				.Select(a => a.i);
			int rank = 1;
			foreach (var item in order)
			{
				prob[item] = Math.Pow(0.9, rank) + 0.1;
				rank++;
			}
		}

		Tree.RootNode CreateChildTree()
		{
			var rand = RandomProvider.GetThreadRandom();
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
			while (rand.NextDouble() < mutationRate)
			{
				next.Mutate(rand);
			}
			next = new ParameterOptimizer(next, target).Run();
			return next;
		}

		int RandomSelect()
		{
			var t = RandomProvider.GetThreadRandom().NextDouble() * prob.Sum();
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
			var val = item.CompareTo(target);
			Debug.Assert(!double.IsNaN(val));
			return val;
		}

	}
}
