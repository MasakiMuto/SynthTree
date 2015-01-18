using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree
{
	public class GAManager
	{
		ItemPool pool;
		public bool Ready { get; private set; }

		public static Random Random { get; private set; }

		public Individual this[int i] { get { return pool[i]; } }

		static GAManager()
		{
			Random = new Random();
		}

		public GAManager()
		{
			Ready = false;
			pool = new ItemPool(9);
		}

		public void Start(Tree.RootNode tree)
		{
			Ready = true;
			pool.Init(tree, 0);
		}

		//public void Start(SynthParam adam)
		//{
		//	Ready = true;
		//	pool.Reset();
		//	eval.Reset();
		//	pool.Init(adam, 0);
		//}

		public void Play(int index)
		{
			if (!Ready) return;
			var param = pool[index];
			param.Play();
		}

		public void Visualize(int index)
		{
			if (!Ready) return;
			var param = pool[index];
			Util.Visualizer.ShowTopology(param.Topology);
			Util.Visualizer.ShowTree(param.Tree);
		}

		public Task PlaySync(int index)
		{
			return Task.Run(()=>Play(index));
		}

		public void Update(IEnumerable<int> saved)
		{
			if (!saved.Any()) return;
			var a = saved.ToArray();
			if (a.Length == 1)
			{
				pool.Init(pool[a[0]].Tree, a[0]);
				//pool.Init(pool[a[0]], a[0]);
				return;
			}
			else
			{
				pool.CrossOver(a);
			}
		}

		public void Save(int index)
		{
			if (!Ready) return;
			System.IO.File.Copy(pool[index].Sound, "result.wav", true);
			pool[index].Tree.Serialize("test.txt");
		}


	}
}
