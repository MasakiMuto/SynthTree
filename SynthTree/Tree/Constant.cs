using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree.Tree
{
	[Serializable]
	public class Constant : TreeBase
	{


		public double Value;

		public Constant(double value)
			: base()
		{
			Value = value;
			NodeType = SynthTree.NodeType.Constant;
		}

		public override void Process()
		{
			AssertChildren(0);
		}

		public override string ToString()
		{
			return "const";
		}

		protected override TreeBase[] CreateChildren()
		{
			return new TreeBase[0];
		}

		protected override TreeBase CloneSelf()
		{
			return new Constant(Value);
		}

		public override void MutateSelf(Random rand)
		{
			base.MutateSelf(rand);
			Value = rand.NextDouble();
		}

		public static double Clamp(double value)
		{
			return Math.Max(-1.0, Math.Min(1.0, value));
		}
	}
}
