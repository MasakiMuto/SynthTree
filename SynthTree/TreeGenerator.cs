﻿using System;
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
		SeriesA		   = 0x040,  //0_0100_0000
		ParallelA	   = 0x041,  //0_0100_0001
		SeriesB		   = 0x020,  //0_0010_0000
		ParallelB	   = 0x021,  //0_0010_0001
		Constant       = 0x000,  //0_0000_0000
		Root           = 0x001,  //0_0000_0001
	}

	public static class TreeGenerator
	{
		public static int MaxLevel = 10;
		static Random random;

		static TreeGenerator()
		{
			random = new Random();
			candidateTypes = Enum.GetValues(typeof(NodeType)).OfType<NodeType>()
				.Where(x => !x.HasFlag(NodeType.FlagAbstract))
				.ToArray();
		}

		static NodeType[] candidateTypes;

		public static TreeBase GetNode(NodeType type, int level)
		{
			var cand = GetCandidate(type, level);
			var r = random.Next(cand.Length);
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
				case NodeType.Split:
					return new FunctionNode<Splitter>();
				case NodeType.Oscil:
					return new FunctionNode<ConstantOscillator>();
				case NodeType.SeriesA:
					return new Modifier(ModifierType.SeriesA1);
				case NodeType.ParallelA:
					return new Modifier(ModifierType.ParallelA1);
				case NodeType.SeriesB:
					return new Modifier(ModifierType.SeriesB1);
				case NodeType.ParallelB:
					return new Modifier(ModifierType.ParallelB1);
				case NodeType.Constant:
					return new Constant(random.NextDouble());
				default:
					throw new ArgumentException();
			}
		}

		/*
		static TreeBase RandomSelect(Options options)
		{
			var r = random.Next(0, 100);
			System.Diagnostics.Debug.Assert(options.Sum(x => x.Key) == 100);
			int s = 0;
			foreach (var item in options)
			{
				s += item.Key;
				if (r < s)
				{
					return item.Value.Value;
				}
			}
			throw new Exception();
		}

		public static RootNode Start()
		{
			return new RootNode().AddChild(TypeA(1)) as RootNode;
		}

		public static TreeBase TypeA(int l)
		{
			return RandomSelect(new Options{
				{50, new LazyT(()=>new FunctionNode<Adder>().AddChildren(CcsA(l+1)))},
				{50, new LazyT(()=>new FunctionNode<Multiplier>().AddChildren(CcsA(l+1)))}
			});
		}

		public static TreeBase CcsA(int l)
		{
			if (l > MaxLevel)
			{
				return DcfA(l + 1);
			}
			return RandomSelect(new Options{
				{33, new LazyT(()=>TypeA(l+1))},
				{33, new LazyT(()=>TmfA(l+1))},
				{34, new LazyT(()=>DcfA(l+1))},
			});

		}

		public static TreeBase DcfA(int l)
		{
			if (l > MaxLevel)
			{
				return new EndA();
			}
			return RandomSelect(new Options{
				{50, new LazyT(()=> new Nop().AddChildren(CcsA(l+1)))},
				{50, new LazyT(()=>new EndA())}
			});
		}

		public static TreeBase TmfA(int l)
		{
			int m = l + 1;
			return RandomSelect(new Options{
				{50, new LazyT(() => new Modifier(ModifierType.SeriesA1).AddChildren(
					CcsA(m), TypeA(m), TypeB(m), CcsW(m), CcsW(m), CcsW(m)
				))},
				{50, new LazyT(()=> new Modifier(ModifierType.ParallelA1).AddChildren(
					CcsA(m), TypeA(m), TypeA(m), TypeB(m), TypeB(m), 
					CcsW(m), CcsW(m), CcsW(m), CcsW(m), CcsW(m), CcsW(m)
				))}
			});
		}

		public static TreeBase TypeB(int l)
		{
			var m = l + 1;
			return RandomSelect(new Options{
				{50, new LazyT(()=> new FunctionNode<Unit.Splitter>().AddChild(CcsB(m)))},
				{50, new LazyT(()=>new FunctionNode<Unit.ConstantOscillator>().AddChildren(
					DcfB(m), NumVal(m), NumVal(m)
				))}
			});
		}

		public static TreeBase CcsB(int l)
		{
			if (l > MaxLevel)
			{
				return DcfB(l + 1);
			}
			return RandomSelect(new Options{
				{33, new LazyT(()=>TypeB(l+1))},
				{33, new LazyT(()=>TmfB(l+1))},
				{34, new LazyT(()=>DcfB(l+1))}
			});
		}

		public static TreeBase DcfB(int l)
		{
			if (l > MaxLevel)
			{
				return new EndA();
			}
			return RandomSelect(new Options{
				{0, new LazyT(()=>new Nop().AddChildren(CcsB(l+1)))},
				{100, new LazyT(()=>new EndA())}
			});
		}

		public static TreeBase TmfB(int l)
		{
			int m = l + 1;
			return RandomSelect(new Options{
				{50, new LazyT(()=>new Modifier(ModifierType.SeriesB1).AddChildren(
					CcsB(m), TypeB(m), TypeA(m), CcsW(m), CcsW(m)
				))},
				{50, new LazyT(()=>new Modifier(ModifierType.ParallelB1).AddChildren(
					CcsB(m), TypeB(m), TypeB(m), TypeA(m), TypeA(m),
					CcsW(m), CcsW(m), CcsW(m), CcsW(m), CcsW(m), CcsW(m)
				))}
			});
		}

		public static TreeBase CcsW(int l)
		{
			if (l > MaxLevel)
			{
				return DcfW(l + 1);
			}
			return RandomSelect(new Options{
				{0, new LazyT(()=>TmfW(l+1))},
				{100, new LazyT(()=>DcfW(l+1))}
			});
		}

		public static TreeBase DcfW(int l)
		{
			if (l > MaxLevel)
			{
				return new EndA();
			}
			return RandomSelect(new Options{
				{50, new LazyT(()=>new Nop().AddChild(CcsW(l+1)))},
				{50, new LazyT(()=>new EndA())}
			});
		}

		public static TreeBase TmfW(int l)
		{
			throw new NotImplementedException();
		}

		public static TreeBase NumVal(int l)
		{
			return new Constant(random.NextDouble());
		}

		*/

	}
}
