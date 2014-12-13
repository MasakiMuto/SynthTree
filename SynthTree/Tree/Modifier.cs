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
		{
			this.type = type;
		}

		public override void Process()
		{
			System.Diagnostics.Debug.Assert((Target is Unit2In1Out && (type == ModifierType.ParallelA1 || type == ModifierType.SeriesA1))
				|| (Target is Unit1In2Out && (type == ModifierType.ParallelB1 || type == ModifierType.SeriesB1)));
			switch (type)
			{
				case ModifierType.SeriesA1:
					SeriesA1();
					break;
				case ModifierType.ParallelA1:
					break;
				case ModifierType.SeriesB1:
					break;
				case ModifierType.ParallelB1:
					break;
				default:
					break;
			}

		}

		void SeriesA1()
		{
			var s1 = Unit2In1Out.CreateStub();
			var s2 = Unit1In2Out.CreateStub();
			Unit.UnitBase.Connect(Target.In[1].FromUnit, 0, s2, 0);
			Unit.UnitBase.Connect(s1, 0, Target.Out[0].ToUnit, 0);
			Unit.UnitBase.Connect(s2, 0, Target, 1);
			Unit.UnitBase.Connect(s2, 1, s1, 1);
			Unit.UnitBase.Connect(Target, 0, s1, 0);
		}
	}

}
