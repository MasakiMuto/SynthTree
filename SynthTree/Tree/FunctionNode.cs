using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree.Tree
{
	public class FunctionNode<T> : TreeBase where T : Unit.UnitBase
	{
		
		Type type;

		public FunctionNode(Type type)
		{
			this.type = type;
		}

		public FunctionNode()
		{
			this.type = typeof(T);
		}

		public FunctionNode(T target, Type type)
			: base()
		{
			Target = target;
			this.type = type;
		}

		public override void Process()
		{
			var obj = type.GetConstructor(new Type[0]).Invoke(new object[0]) as T;
			Unit.UnitBase.Reconnect(Target, obj);
			Target = obj;
			foreach (var item in Children)
			{
				item.Target = obj;
			}
			ProcessChildren();
			//target = obj;
		}

		public override string ToString()
		{
			return type.Name;
		}
	}

}
