using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree
{
	public class DevelopManager
	{

		Unit.UnitBase firstPoint;
		public Unit.Renderer render;
		Tree.RootNode tree;

		public DevelopManager()
		{
			render = CreateEmbryo();
			//tree = CreateInitialTree();
			//Util.Visualizer.ShowTree(tree);
			//tree.Target = firstPoint;
			//foreach (var item in tree.ToBreadthFirstList())
			//{
			//	item.Process();
			//}
			//Util.Visualizer.ShowTopology(render);
			
		}

		public Unit.Renderer CreateEmbryo()
		{
			var src0 = new Unit.TableSource(freqTable);
			var src1 = new Unit.TableSource(powerTable);
			var stub = new Unit.Multiplier();
			var render = new Unit.Renderer();
			Unit.UnitBase.Connect(src0, 0, stub, 0);
			Unit.UnitBase.Connect(src1, 0, stub, 1);
			Unit.UnitBase.Connect(stub, 0, render, 0);
			firstPoint = stub;

			return render;
		}

		static double[] freqTable, powerTable;
		public static int TableLength { get; private set; }

		static DevelopManager()
		{
			SetSource(Enumerable.Repeat(0.11, 4000).ToArray(), Enumerable.Repeat(1.0, 4000).ToArray());
		}

		/// <summary>
		/// ナイキスト周波数で正規化した周波数
		/// </summary>
		/// <param name="freq"></param>
		/// <param name="power"></param>
		public static void SetSource(double[] freq, double[] power)
		{
			freqTable = freq;
			powerTable = power;
			TableLength = Math.Min(freq.Length, powerTable.Length);
		}


		public static Tree.RootNode CreateInitialTree()
		{
			//var root = TreeGenerator.Start();
			var root = new Tree.RootNode();
			root.DevelopChildren();
			root.SetIndex();
			return root;
		}

		public static Unit.Renderer DevelopTopology(Tree.RootNode tree)
		{
			var man = new DevelopManager();
			man.tree = tree;
			tree.Target = man.firstPoint;
			foreach (var item in tree.ToBreadthFirstList())
			{
				item.Process();
			}
			man.render.InitAll();
			return man.render;
		}
	}
}
