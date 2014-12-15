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

		public int Index;


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

		public TreeBase AddChildren(params TreeBase[] items)
		{
			foreach (var item in items)
			{
				AddChild(item);
			}
			return this;
		}

		public IEnumerable<TreeBase> ToBreadthFirstList()
		{
			var ret = new List<TreeBase>();
			var front = new Queue<TreeBase>();
			ToBreadthFirstListInner(ret, front);
			return ret;
		}

		void ToBreadthFirstListInner(List<TreeBase> ret, Queue<TreeBase> front)
		{
			ret.Add(this);
			foreach (var item in Children)
			{
				front.Enqueue(item);
			}
			if (!front.Any())
			{
				return;
			}

			var first = front.Dequeue();
			first.ToBreadthFirstListInner(ret, front);
		}

	}

	public class NopA : TreeBase
	{
		public override void Process()
		{
		}

		public override string ToString()
		{
			return "Nop";
		}
	}

	public class EndA : TreeBase
	{
		public override void Process()
		{
			Target = null;
		}

		public override string ToString()
		{
			return "End";
		}
	}
}
