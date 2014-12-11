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

		public Unit.UnitBase Target;


		public TreeBase()
		{
			Children = new List<TreeBase>();
		}

		protected void ProcessChildren()
		{
			foreach (var item in Children)
			{
				item.Process();
			}
			
		}

		public TreeBase AddChild(TreeBase item)
		{
			item.Parent = this;
			Children.Add(item);
			return this;
		}

		public TreeBase AddChildren(IEnumerable<TreeBase> items)
		{
			foreach (var item in items)
			{
				AddChild(item);
			}
			return this;
		}

		public TreeBase[] ToSingleArray()
		{
			return new[] { this };
		}

		protected static TreeBase RandomSelect(Tuple<Type, int>[] options)
		{
			System.Diagnostics.Debug.Assert(options.Sum(x=>x.Item2) == 100);
			var rand = GAManager.Random.Next(0, 100);
			var s = 0;
			foreach (var item in options)
			{
				s += item.Item2;
				if (rand < s)
				{
					return item.Item1.GetConstructor(System.Type.EmptyTypes).Invoke(null) as TreeBase;
				}
			}
			throw new Exception();
		}
	}

	public class NopA : TreeBase
	{
		public override void Process()
		{
		}
	}

	public class EndA : TreeBase
	{
		public override void Process()
		{
			Target = null;
		}
	}
}
