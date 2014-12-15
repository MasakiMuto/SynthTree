using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

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

		protected void InheritTarget()
		{
			Children[0].Target = this.Target;
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

		[Conditional("DEBUG")]
		protected void AssertChildren(int childCount)
		{
			Debug.Assert(Children.Count == childCount);
		}
	}

	public class NopA : TreeBase
	{
		public override void Process()
		{
			AssertChildren(1);
			InheritTarget();
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
			AssertChildren(0);
			Target = null;
		}

		public override string ToString()
		{
			return "End";
		}
	}
}
