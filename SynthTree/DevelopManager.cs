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
			tree = CreateInitialTree();
			Util.Visualizer.ShowTree(tree);
			tree.Target = firstPoint;
			tree.Process();
			Util.Visualizer.ShowTopology(render);
			
		}

		public Unit.Renderer CreateEmbryo()
		{
			var src0 = new Unit.ConstantSource()
			{
				Value = .008
			};
			var src1 = new Unit.WaveSource();
			var stub = new Unit.Multiplier();
			var render = new Unit.Renderer();
			Unit.UnitBase.Connect(src0, 0, stub, 0);
			Unit.UnitBase.Connect(src1, 0, stub, 1);
			Unit.UnitBase.Connect(stub, 0, render, 0);
			firstPoint = stub;

			return render;
		}


		Tree.RootNode CreateInitialTree()
		{
			var root = TreeGenerator.Start();
			int i = 0;
			var list = root.ToBreadthFirstList();
			foreach (var item in list)
			{
				item.Index = i;
				i++;
			}
			return root;
		}
	}
}
