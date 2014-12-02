using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree.Tree
{
	public abstract class TreeBase
	{
		public TreeBase Parent;
		public List<TreeBase> Children;

		public abstract void Process();
	}
}
