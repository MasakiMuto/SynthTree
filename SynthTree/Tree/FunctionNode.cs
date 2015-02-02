using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree.Tree
{
	[Serializable]
	public class FunctionNode<T> : TreeBase where T : Unit.UnitBase
	{
		static readonly Dictionary<Type, NodeType> typeTable = new Dictionary<Type, NodeType>
		{
			{typeof(Unit.Adder), NodeType.Add},
			{typeof(Unit.Multiplier), NodeType.Mult},
			{typeof(Unit.Splitter), NodeType.Split},
			{typeof(Unit.ConstantOscillator), NodeType.Oscil},
			{typeof(Unit.Filter), NodeType.Filter},
			{typeof(Unit.Delay), NodeType.Delay},
			{typeof(Unit.Parabola), NodeType.Parabola},
			{typeof(Unit.Wire), NodeType.Wire},
		};

		static Type[] typeA, typeB;

		Type type;

		static Dictionary<Type, int> constantCount;

		static FunctionNode()
		{
			typeA = new[]
			{
				typeof(Unit.Adder),
				typeof(Unit.Multiplier), 
			};
			typeB = new[]
			{
				typeof(Unit.Splitter),
				typeof(Unit.ConstantOscillator),
			};
			constantCount = new Dictionary<Type, int>()
			{
				{typeof(Unit.ConstantOscillator), 3},
				{typeof(Unit.Filter), 5},
				{typeof(Unit.Delay), 2},
				{typeof(Unit.Parabola), 3},
			};
			
		}

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
			else if (obj is Unit.Filter)
			{
				AssertChildren(6);
				var o = obj as Unit.Filter;
				o.LpCutoff = GetChildConstant(1);
				o.LpSweep = GetChildConstant(2);
				o.LpResonance = GetChildConstant(3);
				o.HpCutoff = GetChildConstant(4);
				o.HpSweep = GetChildConstant(5);
			}
			else if (obj is Unit.Delay)
			{
				AssertChildren(3);
				var o = obj as Unit.Delay;
				o.Offset = GetChildConstant(1);
				o.Sweep = GetChildConstant(2);
			}
			else if (obj is Unit.Parabola)
			{
				AssertChildren(4);
				var o = obj as Unit.Parabola;
				o.A = GetChildConstant(1);
				o.B = GetChildConstant(2);
				o.C = GetChildConstant(3);
			}
			else
			{
				AssertChildren(1);
			}

		}

		double GetChildConstant(int i)
		{
			return (Children[i] as Constant).Value;
		}

		public override string ToString()
		{
			return type.Name;
		}

		protected override TreeBase[] CreateChildren()
		{
			NodeType head;
			int constant = 0;
			if (Level > MaxLevel)
			{
				head = NodeType.End;
			}
			else if (typeA.Contains(type))
			{
				head = SynthTree.NodeType.FlagA;
			}
			else if(typeB.Contains(type))
			{
				head = SynthTree.NodeType.FlagB;
			}
			else
			{
				head = SynthTree.NodeType.FlagW;
			}
			constantCount.TryGetValue(type, out constant);

			return Enumerable.Repeat(head, 1).Concat(Enumerable.Repeat(NodeType.Constant, constant))
				.Select(x => TreeGenerator.GetNode(x, Level))
				.ToArray();
		}

		protected override TreeBase CloneSelf()
		{
			return new FunctionNode<T>();
		}
	}

}
