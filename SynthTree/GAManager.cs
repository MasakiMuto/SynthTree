using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree
{
	public class GAManager
	{
		public ItemPool Pool { get; private set; }
		public bool Ready { get; private set; }

		public static Random Random { get; private set; }

		public Individual this[int i] { get { return Pool[i]; } }

		static GAManager()
		{
			Random = new Random();
		}

		public GAManager()
		{
			Ready = false;
			Pool = new ItemPool(9);
		}

		public void Start(Tree.RootNode tree)
		{
			Ready = true;
			Pool.Init(tree);
		}

		public void Play(int index)
		{
			if (!Ready) return;
			var param = Pool[index];
			param.Play();
		}

		public void Visualize(int index)
		{
			if (!Ready) return;
			var param = Pool[index];
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
				Pool.MutateAll(a[0]);
				return;
			}
			else
			{
				Pool.CrossOver(a);
			}
		}

		public void Save(int index)
		{
			if (!Ready) return;
			Pool[index].SaveSound();
			System.IO.File.Copy(Pool[index].Sound, "result.wav", true);
			Pool[index].Tree.Serialize("test.txt");
		}


	}
}
