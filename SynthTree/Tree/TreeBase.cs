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

		protected int Level { get; private set; }

		protected static readonly int MaxLevel = 5;

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
			item.Level = this.Level + 1;
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

		public void DevelopChildren()
		{
			var children = CreateChildren();
			AddChildren(children);
			foreach (var item in children)
			{
				item.DevelopChildren();
			}
		}

		protected abstract TreeBase[] CreateChildren();


	}

	public class Nop : TreeBase
	{
		public enum ConnectionType
		{
			A,
			B,
			W
		}

		ConnectionType type;

		public Nop(ConnectionType type)
			: base()
		{
			this.type = type;
		}

		public override void Process()
		{
			AssertChildren(1);
			InheritTarget();
		}

		public override string ToString()
		{
			return "Nop";
		}

		protected override TreeBase[] CreateChildren()
		{
			NodeType nt;
			switch (type)
			{
				case ConnectionType.A:
					nt = NodeType.FlagA;
					break;
				case ConnectionType.B:
					nt = NodeType.FlagB;
					break;
				case ConnectionType.W:
					nt = NodeType.FlagW;
					break;
				default:
					throw new Exception();
			}
			return new[] { TreeGenerator.GetNode(nt, Level) };
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

		protected override TreeBase[] CreateChildren()
		{
			return new TreeBase[0];
		}
	}
}
