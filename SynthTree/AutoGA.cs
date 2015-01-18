using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioLib;

namespace SynthTree
{
	public class AutoGA
	{
		Analyzer target;
		Individual[] items;
		double[] scores;
		double[] prob;

		int poolSize = 50;
		readonly int Elite = 5;
		readonly double Mutation = 0.2;

		public int Generation { get; private set; }

		Random rand;

		public Individual BestElite { get; private set; }

		public Action OnUpdate;

		public AutoGA(string targetFile, int poolSize)
		{
			rand = new Random();
			this.poolSize = poolSize;

			target = new Analyzer(targetFile);
			target.CalcSpectrogram();

			items = new Individual[poolSize];
			
			scores = new double[poolSize];
			prob = new double[poolSize];
		}

		public void Init()
		{
			for (int i = 0; i < poolSize; i++)
			{
				items[i] = new Individual(DevelopManager.CreateInitialTree());
			}
			Generation = 0;
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
			for (int i = 0; i < poolSize; i++)
			{
				scores[i] = Eval(items[i]);
			}
			var elite = scores.Select((x, i) => new { x, i })
				.OrderBy(a => a.x)
				.Take(Elite)
				.Select(a => a.i).ToArray();
			var best = scores[elite[0]];
			BestElite = items[elite[0]];
			var s = scores.Sum();
			for (int i = 0; i < poolSize; i++)
			{
				prob[i] = best / scores[i];
			}

			Individual[] newItems = new Individual[poolSize];
			for (int i = 0; i < elite.Length; i++)
			{
				newItems[i] = items[elite[i]];
			}
			for (int i = elite.Length; i < poolSize; i++)
			{
				newItems[i] = CreateChild();
			}
		}

		Individual CreateChild()
		{
			int p1, p2;
			p1 = RandomSelect();
			do
			{
				p2 = RandomSelect();
			} while (p2 != p1);
			var next = items[p1].Tree.CloneTree();
			next.CrossOver(items[p2].Tree, rand);
			if (rand.NextDouble() < Mutation)
			{
				//next.Mutate(rand);
			}
			return new Individual(next);
		}

		int RandomSelect()
		{
			var t = rand.NextDouble() * prob.Sum();
			var s = 0.0;
			for (int i = 0; i < poolSize; i++)
			{
				s += prob[i];
				if (s > t)
				{
					return i;
				}
			}
			return poolSize - 1;
		}


		double Eval(Individual item)
		{
			return item.CompareTo(target);
		}

	}
}
