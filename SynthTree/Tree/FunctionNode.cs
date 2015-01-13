using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree.Tree
{
	public class FunctionNode<T> : TreeBase where T : Unit.UnitBase
	{
		static readonly Dictionary<Type, NodeType> typeTable = new Dictionary<Type, NodeType>
		{
			{typeof(Unit.Adder), NodeType.Add},
			{typeof(Unit.Multiplier), NodeType.Mult},
			{typeof(Unit.Splitter), NodeType.Split},
			{typeof(Unit.ConstantOscillator), NodeType.Oscil}
		};

		Type type;

		public FunctionNode()
			: base()
		{
			this.type = typeof(T);
			NodeType = typeTable[type];
		}

		public override void Process()
		{
			var obj = type.GetConstructor(new Type[0]).Invoke(new object[0]) as T;
			Unit.UnitBase.Reconnect(Target, obj);
			Target = obj;
			InheritTarget();
			if (obj is Unit.ConstantOscillator)
			{
				AssertChildren(4);
				var o = obj as Unit.ConstantOscillator;
				o.Constant = (Children[1] as Constant).Value;
				o.Phase = (Children[2] as Constant).Value;
				o.WaveFormValue = (Children[3] as Constant).Value;
			}
			else
			{
				AssertChildren(1);
			}

		}

		public override string ToString()
		{
			return type.Name;
		}

		protected override TreeBase[] CreateChildren()
		{
			//if (Level > MaxLevel)
			//{
			//	return new[] { TreeGenerator.GetNode(NodeType.End) };
			//}
			if (type == typeof(Unit.ConstantOscillator))
			{
				return new[] { Level > MaxLevel ? NodeType.End : NodeType.FlagB, NodeType.Constant, NodeType.Constant, NodeType.Constant }
					.Select(x => TreeGenerator.GetNode(x, Level))
					.ToArray();
			}
			else if (Level > MaxLevel)
			{
				return new[] { TreeGenerator.GetNode(NodeType.End, Level) };
			}
			else if (type == typeof(Unit.Adder) || type == typeof(Unit.Multiplier))
			{
				return new[] { TreeGenerator.GetNode(NodeType.FlagA, Level) };
			}
			else if(type == typeof(Unit.Splitter))
			{
				return new[] { TreeGenerator.GetNode(NodeType.FlagB, Level) };
			}
			throw new Exception();
		}

		protected override TreeBase CloneSelf()
		{
			return new FunctionNode<T>();
		}
	}

}
