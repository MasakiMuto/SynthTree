using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SynthTree.Tree;
using SynthTree.Unit;

using LazyT = System.Lazy<SynthTree.Tree.TreeBase>;
	
namespace SynthTree
{
	using Syntax = Func<int, TreeBase>;
	using Option = KeyValuePair<int, LazyT>;
	using Options = Util.KeyValueSet<int, LazyT>;
	
	public static class TreeGenerator
	{
		public static int MaxLevel = 10;
		static Random random;

		static TreeGenerator()
		{
			random = new Random();
		}

		static TreeBase RandomSelect(Options options)
		{
			var r = random.Next(0, 100);
			System.Diagnostics.Debug.Assert(options.Sum(x=>x.Key) == 100);
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
				{50, new LazyT(()=> new NopA().AddChildren(CcsA(l+1)))},
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
				return DcfB(l+1);
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
				{0, new LazyT(()=>new NopA().AddChildren(CcsB(l+1)))},
				{100, new LazyT(()=>new EndA())}
			});
		}

		public static TreeBase TmfB(int l)
		{
			int m = l+1;
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
				{50, new LazyT(()=>new NopA().AddChild(CcsW(l+1)))},
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


		
	}
}
