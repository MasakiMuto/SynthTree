using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SynthTree.Tree;
using SynthTree.Unit;
using System.Diagnostics;

using LazyT = System.Lazy<SynthTree.Tree.TreeBase>;

namespace SynthTree
{
	using Syntax = Func<int, TreeBase>;
	using Option = KeyValuePair<int, LazyT>;
	using Options = Util.KeyValueSet<int, LazyT>;

	[Flags]
	public enum NodeType//(Abstract)_(Type)(AorBorW)_(SpecifiedValue^4)
	{
		Mask           = 0x1f0,  //1_1111_0000

		FlagAbstract   = 0x100,  //1_0000_0000 
		FlagType       = 0x180,  //1_1000_0000
		FlagGeneral    = 0x170,  //1_0111_0000
		FlagA		   = 0x140,  //1_0100_0000
		FlagB		   = 0x120,  //1_0010_0000
		FlagW		   = 0x110,  //1_0001_0000

		NopA		   = 0x042,  //0_0100_0010
		NopB           = 0x022,  //0_0010_0010
		NopW           = 0x012,  //0_0001_0010
		End			   = 0x071,  //0_0111_0001
		Add			   = 0x0c0,  //0_1100_0000
		Mult		   = 0x0c1,  //0_1100_0001
		Split		   = 0x0a0,  //0_1010_0000
		Oscil		   = 0x0a1,  //0_1010_0001
		Filter         = 0x092,  //0_1001_0010
		Delay          = 0x094,  //0_1001_0100
		Parabola       = 0x098,  //0_1001_1000
		Wire           = 0x093,  //0_1001_0011
		SeriesA		   = 0x040,  //0_0100_0000
		ParallelA	   = 0x041,  //0_0100_0001
		UnitA          = 0x044,  //0_0100_0100 
		SeriesB		   = 0x020,  //0_0010_0000
		ParallelB	   = 0x021,  //0_0010_0001
		UnitB          = 0x024,  //0_0010_0100
		LoopW = 0x010,  //0_0001_0000
		UnitW          = 0x011,  //0_0001_0001
		Constant       = 0x000,  //0_0000_0000
		Root           = 0x001,  //0_0000_0001
	}

	public static class TreeGenerator
	{
		public static int MaxLevel = 10;

		static TreeGenerator()
		{
			candidateTypes = Enum.GetValues(typeof(NodeType)).OfType<NodeType>()
				.Where(x => !x.HasFlag(NodeType.FlagAbstract))
				.ToArray();
		}

		static NodeType[] candidateTypes;

		public static TreeBase GetNode(NodeType type, int level)
		{
			var cand = GetCandidate(type, level);
			var r = RandomProvider.GetThreadRandom().Next(cand.Length);
			return CreateNode(cand[r]);
		}

		public static NodeType[] GetCandidate(NodeType type, int level)
		{
			if (!type.HasFlag(NodeType.FlagAbstract))
			{
				return new[]{ type };
			}
			type -= NodeType.FlagAbstract;
			if (level > MaxLevel && !type.HasFlag(NodeType.FlagType))
			{
				return new[] { NodeType.End };
			}
			return candidateTypes.Where(x => (x & type) == type).ToArray();
		}

		static TreeBase CreateNode(NodeType type)
		{
			Debug.Assert(!type.HasFlag(NodeType.FlagAbstract));
			ModifierType mt;
			if (ModifierTable.TryGetValue(type, out mt))
			{
				return new Modifier(mt);
			}
			switch (type)
			{
				case NodeType.NopA:
					return new Nop(Nop.ConnectionType.A);
				case NodeType.NopB:
					return new Nop(Nop.ConnectionType.B);
				case NodeType.NopW:
					return new Nop(Nop.ConnectionType.W);
				case NodeType.End:
					return new EndA();
				case NodeType.Add:
					return new FunctionNode<Adder>();
				case NodeType.Mult:
					return new FunctionNode<Multiplier>();
				case NodeType.Filter:
					return new FunctionNode<Filter>();
				case NodeType.Delay:
					return new FunctionNode<Delay>();
				case NodeType.Split:
					return new FunctionNode<Splitter>();
				case NodeType.Oscil:
					return new FunctionNode<ConstantOscillator>();
				case NodeType.Parabola:
					return new FunctionNode<Parabola>();
				case NodeType.Wire:
					return new FunctionNode<Wire>();
				case NodeType.Constant:
					return new Constant(RandomProvider.GetThreadRandom().NextDouble() * 2.0 - 1.0);
				default:
					throw new ArgumentException();
			}
		}

		public static readonly Dictionary<NodeType, ModifierType> ModifierTable = new Dictionary<NodeType,ModifierType>
		{
			{NodeType.SeriesA, ModifierType.SeriesA},
			{NodeType.SeriesB, ModifierType.SeriesB},
			{NodeType.ParallelA, ModifierType.ParallelA},
			{NodeType.ParallelB, ModifierType.ParallelB},
			{NodeType.LoopW, ModifierType.LoopW},
			{NodeType.UnitA, ModifierType.UnitA},
			{NodeType.UnitB, ModifierType.UnitB},
			{NodeType.UnitW, ModifierType.UnitW},
		};

	}
}
