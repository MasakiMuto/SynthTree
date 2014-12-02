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

		public FunctionNode(T target, Type type)
			: base()
		{
			this.target = target;
			this.type = type;
		}

		public override void Process()
		{
			var obj = type.GetConstructor(new Type[0]).Invoke(new object[0]) as T;
			Unit.UnitBase.Reconnect(target, obj);
			//target = obj;
		}
	}
}
