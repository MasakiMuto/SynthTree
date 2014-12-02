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
			ProcessChildren();
		}
	}
}
