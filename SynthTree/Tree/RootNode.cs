using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree.Tree
{
	[Serializable]
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

		TreeBase GetRandomChildWithoutConstant(Random rand, bool containSelf)
		{
			var ar = ToBreadthFirstList().Where(x => x.NodeType != SynthTree.NodeType.Constant).ToArray();
			return ar[rand.Next(containSelf ? 0 : 1, ar.Length)];
		}

		/// <summary>
		/// 自分自身を突然変異させる
		/// </summary>
		/// <param name="rand"></param>
		public void Mutate(Random rand)
		{
			var target = GetRandomChild(rand, true);
			target.MutateSelf(rand);
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
			var target = GetRandomChildWithoutConstant(rand, false);
			var type = target.NodeType & SynthTree.NodeType.Mask;
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

		public void Serialize(string fn)
		{
			IFormatter f = new BinaryFormatter();
			using (var file = new FileStream(fn, FileMode.Create, FileAccess.Write))
			{
				f.Serialize(file, this);
			}
		}

		public static RootNode Deserialize(string fn)
		{
			IFormatter f = new BinaryFormatter();
			using (var file = new FileStream(fn, FileMode.Open, FileAccess.Read))
			{
				return f.Deserialize(file) as RootNode;
			}
		}


	}
}
