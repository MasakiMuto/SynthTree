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

		public TreeBase AddChildren(params TreeBase[] items)
		{
			foreach (var item in items)
			{
				AddChild(item);
			}
			return this;
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
