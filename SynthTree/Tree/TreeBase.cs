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
		public TreeBase Parent { get; private set; }
		public List<TreeBase> Children { get; private set; }

		public abstract void Process();

		public Unit.UnitBase Target;

		public int Index;

		protected int Level { get; private set; }

		protected static readonly int MaxLevel = 5;

		public NodeType NodeType { get; protected set; }

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

		protected abstract TreeBase CloneSelf();

		protected TreeBase Clone()
		{
			var c = CloneSelf();
			foreach (var item in Children)
			{
				c.AddChild(item.Clone());
			}
			return c;
		}

		public void ChangeChild(TreeBase target, TreeBase next)
		{
			var i = Children.IndexOf(target);
			Children[i] = next;
			next.Parent = this;
			next.Level = this.Level + 1;
		}

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
			switch (type)
			{
				case ConnectionType.A:
					NodeType = SynthTree.NodeType.NopA;
					break;
				case ConnectionType.B:
					NodeType = SynthTree.NodeType.NopB;
					break;
				case ConnectionType.W:
					NodeType = SynthTree.NodeType.NopW;
					break;
				default:
					break;
			}
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

		protected override TreeBase CloneSelf()
		{
			var c = new Nop(type);
			return c;
		}

	}

	public class EndA : TreeBase
	{
		public EndA()
		{
			NodeType = SynthTree.NodeType.End;
		}

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

		protected override TreeBase CloneSelf()
		{
			return new EndA();
		}
	}
}
