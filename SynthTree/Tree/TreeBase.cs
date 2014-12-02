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

		public TreeBase()
		{
			Children = new List<TreeBase>();
		}

		protected void ProcessChildren()
		{
			if (Children == null)
			{
				return;
			}
			foreach (var item in Children)
			{
				item.Process();
			}
			
		}

		public void AddChild(TreeBase item)
		{
			item.Parent = this;
			Children.Add(item);
		}
	}
}
