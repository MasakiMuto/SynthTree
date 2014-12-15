using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree.Tree
{
	public class RootNode : TreeBase
	{


		public override void Process()
		{
			foreach (var item in Children)
			{
				item.Target = this.Target;
			}
			ProcessChildren();
		}

		public RootNode Mutate()
		{
			throw new NotImplementedException();
		}

		public RootNode CrossOver(RootNode another)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return "Root";
		}
	}
}
