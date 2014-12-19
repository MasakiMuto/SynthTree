using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree.Tree
{
	public class RootNode : TreeBase
	{
		public RootNode()
		{
			NodeType = SynthTree.NodeType.Root;
		}

		public override void Process()
		{
			AssertChildren(1);
			InheritTarget();
		}

		TreeBase GetRandomChild(Random rand, bool containSelf)
		{
			var children = ToBreadthFirstList().ToArray();
			return children[rand.Next(containSelf ? 0 : 1, children.Length)];
		}

		/// <summary>
		/// 自分自身を突然変異させる
		/// </summary>
		/// <param name="rand"></param>
		public void Mutate(Random rand)
		{
			var target = GetRandomChild(rand, true);
			target.Children.Clear();
			target.DevelopChildren();
			SetIndex();
		}

		/// <summary>
		/// 自分自身を他個体と交叉させる
		/// </summary>
		/// <param name="another"></param>
		/// <returns>交叉が行えなかったらfalse</returns>
		public bool CrossOver(RootNode another, Random rand)
		{
			another = another.CloneTree();
			var target = GetRandomChild(rand, false);
			var type = target.NodeType;
			type = (type & SynthTree.NodeType.Mask);
			var candidate = another.ToBreadthFirstList()
				.Where(x => (type & x.NodeType) == type).ToArray();
			if (!candidate.Any())
			{
				return false;
			}
			var t2 = candidate[rand.Next(candidate.Length)];
			var t2p = t2.Parent;
			target.Parent.ChangeChild(target, t2);
			t2p.ChangeChild(t2, target);
			SetIndex();
			return true;
		}

		public void SetIndex()
		{
			int i = 0;
			foreach (var item in ToBreadthFirstList())
			{
				item.Index = i;
				i++;
			}
		}

		protected override TreeBase CloneSelf()
		{
			return new RootNode();
		}

		public RootNode CloneTree()
		{
			var r = Clone() as RootNode;
			r.SetIndex();
			return r;
		}

		public override string ToString()
		{
			return "Root";
		}

		protected override TreeBase[] CreateChildren()
		{
			return new[] { TreeGenerator.GetNode(NodeType.FlagA | NodeType.FlagType, Level) };
		}


	}
}
