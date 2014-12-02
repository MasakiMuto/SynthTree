using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree.Tree
{
	public class FunctionNode<T> : TreeBase where T : Unit.UnitBase
	{
		T target;

		Type type;

		

		public override void Process()
		{
			target = type.GetConstructor(new Type[0]).Invoke(new object[0]) as T;
		}
	}
}
