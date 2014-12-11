using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nodes = System.Collections.Generic.IEnumerable<SynthTree.Tree.TreeBase>;
using LazyT = System.Lazy<System.Collections.Generic.IEnumerable<SynthTree.Tree.TreeBase>>;
	
namespace SynthTree.Tree
{
	using Syntax = Func<int, Nodes>;

	struct Option
	{
		public readonly int Probability;
		public readonly LazyT Action;
		public Option(int p, LazyT a)
		{
			Probability = p;
			Action = a;
		}
	}

	class Options : IEnumerable<Option>
	{
		List<Option> items;

		public Options()
		{
			items = new List<Option>();
		}

		public void Add(int p, LazyT a)
		{
			items.Add(new Option(p, a));
		}

		public IEnumerator<Option> GetEnumerator()
		{
			return items.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return items.GetEnumerator();
		}
	}

	public static class Generator
	{
		public static int MaxLevel = 10;
		static Random random;

		static Generator()
		{
			random = new Random();
		}

		static Nodes RandomSelect(Options options)
		{
			var r = random.Next(0, 100);
			System.Diagnostics.Debug.Assert(options.Sum(x=>x.Probability) == 100);
			int s = 0;
			foreach (var item in options)
			{
				s += item.Probability;
				if (r < s)
				{
					return item.Action.Value;
				}
			}
			throw new Exception();
		}

		public static RootNode Start()
		{
			return new RootNode().AddChildren(TypeA(1)) as RootNode;
		}

		public static Nodes TypeA(int l)
		{
			return RandomSelect(new Options{
				{50, new LazyT(()=>new FunctionNode<Unit.Adder>().AddChildren(CcsA(l+1)).ToSingleArray())},
				{50, new LazyT(()=>new FunctionNode<Unit.Multiplier>().AddChildren(CcsA(l+1)).ToSingleArray())}
			});
		}

		public static Nodes CcsA(int l)
		{
			return RandomSelect(new Options{
				{0, new LazyT(()=>TypeA(l+1))},
				{0, new LazyT(()=>TmfA(l+1))},
				{100, new LazyT(()=>DcfA(l+1))},
			});
			
		}

		public static Nodes DcfA(int l)
		{
			return RandomSelect(new Options{
				{50, new LazyT(()=> new NopA().AddChildren(CcsA(l+1)).ToSingleArray())},
				{50, new LazyT(()=>new EndA().ToSingleArray())}
			});
		}

		public static Nodes TmfA(int l)
		{
			throw new NotImplementedException();
		}


		
	}
}
