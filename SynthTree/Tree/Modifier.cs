using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SynthTree.Unit;

namespace SynthTree.Tree
{
	public enum ModifierType
	{
		SeriesA1,
		ParallelA1,
		SeriesB1,
		ParallelB1,
	}

	public class Modifier : TreeBase
	{
		readonly ModifierType type;

		public Modifier(ModifierType type)
			: base()
		{
			this.type = type;
			switch (type)
			{
				case ModifierType.SeriesA1:
					NodeType = SynthTree.NodeType.SeriesA;
					break;
				case ModifierType.ParallelA1:
					NodeType = SynthTree.NodeType.ParallelA;
					break;
				case ModifierType.SeriesB1:
					NodeType = SynthTree.NodeType.SeriesB;
					break;
				case ModifierType.ParallelB1:
					NodeType = SynthTree.NodeType.ParallelB;
					break;
				default:
					break;
			}
		}

		public override void Process()
		{
			System.Diagnostics.Debug.Assert((Target is Unit2In1Out && (type == ModifierType.ParallelA1 || type == ModifierType.SeriesA1))
				|| (Target is Unit1In2Out && (type == ModifierType.ParallelB1 || type == ModifierType.SeriesB1)));
			InheritTarget();
			switch (type)
			{
				case ModifierType.SeriesA1:
					SeriesA1();
					break;
				case ModifierType.ParallelA1:
					ParallelA1();
					break;
				case ModifierType.SeriesB1:
					SeriesB1();
					break;
				case ModifierType.ParallelB1:
					ParallelB1();
					break;
				default:
					throw new Exception();
			}

		}

		public override string ToString()
		{
			return type.ToString();
		}

		void SeriesA1()
		{
			var a = Unit2In1Out.CreateStub();
			var b = Unit1In2Out.CreateStub();
			Unit.UnitBase.Connect(b, 1, a, 1);
			UnitBase.ReplaceInput(Target, 1, b, 0, b, 0);
			UnitBase.ReplaceOutput(Target, 0, a, 0, a, 0);

			Children[1].Target = a;
			Children[2].Target = b;
		}

		void ParallelA1()
		{
			var a1 = Unit2In1Out.CreateStub();
			var a2 = Unit2In1Out.CreateStub();
			var b1 = Unit1In2Out.CreateStub();
			var b2 = Unit1In2Out.CreateStub();

			UnitBase.ReplaceInput(Target, 0, b1, 0, a1, 0);
			UnitBase.ReplaceInput(Target, 1, b2, 0, a2, 0);
			UnitBase.Connect(b1, 0, a1, 0);
			UnitBase.Connect(b1, 1, a2, 0);
			UnitBase.Connect(b2, 0, a1, 1);
			UnitBase.Connect(b2, 1, a2, 1);

			Children[1].Target = a1;
			Children[2].Target = a2;
			Children[3].Target = b1;
			Children[4].Target = b2;
		}

		void SeriesB1()
		{
			var a = Unit2In1Out.CreateStub();
			var b = Unit1In2Out.CreateStub();
			UnitBase.ReplaceOutput(Target, 0, b, 0, b, 0);
			UnitBase.ReplaceOutput(Target, 1, a, 0, a, 1);
			UnitBase.Connect(b, 1, a, 0);

			Children[1].Target = b;
			Children[2].Target = a;
		}

		void ParallelB1()
		{
			var a0 = Unit2In1Out.CreateStub();
			var a1 = Unit2In1Out.CreateStub();
			var b0 = Unit1In2Out.CreateStub();
			var b1 = Unit1In2Out.CreateStub();

			UnitBase.ReplaceOutput(Target, 0, a0, 0, b0, 0);
			UnitBase.ReplaceOutput(Target, 1, a1, 0, b1, 0);
			UnitBase.Connect(b0, 0, a0, 0);
			UnitBase.Connect(b1, 1, a1, 1);
			UnitBase.Connect(b0, 1, a1, 0);
			UnitBase.Connect(b1, 0, a0, 1);

			Children[1].Target = b0;
			Children[2].Target = b1;
			Children[3].Target = a0;
			Children[4].Target = a1;
		}

		protected override TreeBase[] CreateChildren()
		{
			NodeType[] nt;
			int wc;
			var ta = NodeType.FlagA | NodeType.FlagType;
			var tb = NodeType.FlagB | NodeType.FlagType;
			var fa = Level > MaxLevel ? NodeType.End : NodeType.FlagA;
			var fb = Level > MaxLevel ? NodeType.End : NodeType.FlagB;
			switch (this.type)
			{
				case ModifierType.SeriesA1:
					nt = new[] { fa, ta, tb };
					wc = 3;
					break;
				case ModifierType.ParallelA1:
					nt = new[]{ fa, ta, ta, tb, tb};
					wc = 6;
					break;
				case ModifierType.SeriesB1:
					nt = new []{fb, tb, ta };
					wc = 3;
					break;
				case ModifierType.ParallelB1:
					nt = new[] { fb, tb, tb, ta, ta };
					wc = 6;
					break;
				default:
					throw new Exception();
			}
			
			return nt/*.Concat(Enumerable.Repeat(NodeType.FlagW, wc))*/.Select(x=>TreeGenerator.GetNode(x, Level)).ToArray();
		}

		protected override TreeBase CloneSelf()
		{
			return new Modifier(type);
		}
	}

}
