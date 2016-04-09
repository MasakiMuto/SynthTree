using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree
{

	public class ParameterOptimizer
	{
		Tree.Constant[] items;

		readonly int Dimension;
		readonly AudioLib.Analyzer Target;
		readonly Tree.RootNode Original;
		Dictionary<double[], Individual> history;

		readonly int PoolSize = 7;
		readonly int GenSize = 5;
		int generation;

		double[][] pool;
		Random rand;

		public ParameterOptimizer(Tree.RootNode original, AudioLib.Analyzer target)
		{
			rand = RandomProvider.GetThreadRandom();
			Original = original;
			items = GetActiveConstants(original).ToArray();
			Dimension = items.Length;
			Target = target;
		
			pool = new double[PoolSize][];
			pool[0] = items.Select(x => x.Value).ToArray();
			for (int i = 1; i < PoolSize; i++)
			{
				pool[i] = CreateVector();
			}

			history = new Dictionary<double[], Individual>();
			probTable = new double[PoolSize];
			scores = new double[PoolSize];
		}

		IEnumerable<Tree.Constant> GetActiveConstants(Tree.RootNode tree)
		{
			return tree.ToBreadthFirstList().OfType<Tree.Constant>().Where(x => x.Parent.IsUsed());
		}

		public Tree.RootNode Run()
		{
			if (Dimension == 0)
			{
				return VectorToTree(pool[0]);
			}
			for (int i = 0; i < GenSize; i++)
			{
				generation++;
				Update();
			}
			return VectorToTree(pool[0]);
		}

		Tree.RootNode VectorToTree(double[] vector)
		{
			var tree = Original.CloneTree();
			int i = 0;
			foreach (var c in GetActiveConstants(tree))
			{
				c.Value = vector[i];
				i++;
			}
			return tree;
		}

		double[] scores;

		void Update()
		{
			for (int i = 0; i < PoolSize; i++)
			{
				scores[i] = Eval(pool[i]);
			}

			CreateProbTable();
			double[][] next = new double[PoolSize][];
			next[0] = pool[GetEliteIndex()];
			for (int i = 1; i < PoolSize; i++)
			{
				next[i] = CreateNext();
			}
			pool = next;
		
		}

		

		double[] CreateNext()
		{
			var p1 = RandomSelect();
			int p2;
			do
			{
				p2 = RandomSelect();
			} while (p2 == p1);
			var n = CrossOver(pool[p1], pool[p2]);
			Mutate(n);
			return n;
		}

		int RandomSelect()
		{
			var r = rand.NextDouble() * probTable.Sum();
			var s = 0.0;
			for (int i = 0; i < probTable.Length; i++)
			{
				s += probTable[i];
				if(r < s)
				{
					return i;
				}
			}
			return probTable.Length - 1;
		}

		int GetEliteIndex()
		{
			double m = double.MaxValue;
			int j = -1;
			for (int i = 0; i < PoolSize; i++)
			{
				if (scores[i] < m)
				{
					j = i;
					m = scores[i];
				}
			}
			return j;
		}

		double[] probTable;

		void CreateProbTable()
		{
			var o = scores.Select((x, i) => new { x, i })
				.OrderBy(a => a.x)
				.Select(a => a.i);
			int j = 1;
			foreach (var item in o)
			{
				probTable[item] = Math.Pow(0.6, j);
				j++;
			}
		}

		double[] CreateVector()
		{
			return Enumerable.Range(0, Dimension).Select(x => rand.NextDouble()).ToArray();
		}

		double[] CrossOver(double[] p1, double[] p2)
		{
			var p = rand.Next(Dimension);
			var r = new double[Dimension];
			for (int i = 0; i < Dimension; i++)
			{
				r[i] = i < p ? p1[i] : p2[i];
			}
			return r;
		}

		const double MutateProb = 0.2;
		const double MutateScale = 0.2;

		double[] Mutate(double[] p)
		{
			for (int i = 0; i < Dimension; i++)
			{
				if(rand.NextDouble() < MutateProb)
				{
					p[i] += (MutateScale * 2 - MutateScale) * rand.NextDouble();
					p[i] = Tree.Constant.Clamp(p[i]);
				}
			}
			return p;
		}

		double Eval(double[] param)
		{
			
			Individual ind;
			if (!history.TryGetValue(param, out ind))
			{
				var tree = VectorToTree(param);
				ind = new Individual(tree, false);
				history[param] = ind;
			}
			return ind.CompareTo(Target);
		}
	}
}
