using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree
{
	public class DevelopManager
	{
		int stubIndex;

		public DevelopManager()
		{

		}

		public Unit.Renderer CreateEmbryo()
		{
			var src0 = new Unit.ConstantSource()
			{
				Value = .008
			};
			var src1 = new Unit.WaveSource();
			var stub = new Unit.UnitStub2In1Out(stubIndex);
			stubIndex++;
			var render = new Unit.Renderer();
			Unit.UnitBase.Connect(src0, 0, stub, 0);
			Unit.UnitBase.Connect(src1, 0, stub, 1);
			Unit.UnitBase.Connect(stub, 0, render, 0);


			var root = new Tree.RootNode();
			root.AddChild(new Tree.FunctionNode<Unit.Unit2In1Out>(stub, typeof(Unit.Multiplier)));
			root.Process();
			
			return render;

		}

		void CreateInitialTree()
		{
			
		}
	}
}
