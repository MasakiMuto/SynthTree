using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree.Tree
{
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
			return "const" + Value.ToString().Replace(".", "_"); ;
		}

		protected override TreeBase[] CreateChildren()
		{
			return new TreeBase[0];
		}

		protected override TreeBase CloneSelf()
		{
			return new Constant(Value);
		}
	}
}
