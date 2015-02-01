using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace SynthTree.Util
{
	public static class Visualizer
	{
		public static void ShowTree(Tree.RootNode root)
		{
			var scriptName = "tree.dot";
			var fileName = "tree.png";
			CreateTreeScript(root, scriptName);
			RunGraphviz(scriptName, fileName);
			Process.Start(fileName);
		}

		public static void ShowTopology(Unit.Renderer renderer)
		{
			var scriptName = "topo.dot";
			var fileName = "topo.png";
			CreateTopologyScript(renderer, scriptName);
			RunGraphviz(scriptName, fileName);
			Process.Start(fileName);
		}

		static void RunGraphviz(string scriptName, string fileName)
		{
			using (var p = new Process())
			{
				p.StartInfo.FileName = @"release\bin\dot.exe";
				p.StartInfo.Arguments = string.Format("-Tpng {0} -o {1}", scriptName, fileName);
				p.StartInfo.RedirectStandardError = true;
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.UseShellExecute = false;
				p.Start();
				p.WaitForExit();
			}
		}

		static void CreateTreeScript(Tree.RootNode root, string fileName)
		{
			var str = new StringBuilder();
			str.AppendFormat("digraph {0} {{ \n", Path.GetFileNameWithoutExtension(fileName));
			foreach (var item in root.ToBreadthFirstList())
			{
				str.AppendFormat("\t{0}_{1}[label=\"{0}\"];\n", item, item.Index);
				PrintTreeNode(item, str);
			}
			str.AppendLine("}");
			File.WriteAllText(fileName, str.ToString());
			
		}

		static void PrintTreeNode(Tree.TreeBase node, StringBuilder builder)
		{
			foreach (var item in node.Children)
			{
				builder.AppendFormat("\t{0}_{2} -> {1}_{3};\n", node, item, node.Index, item.Index);
			}
		}

		static void CreateTopologyScript(Unit.Renderer renderer, string fileName)
		{
			var builder = new StringBuilder();
			builder.AppendFormat("digraph {0} {{\n", Path.GetFileNameWithoutExtension(fileName));
			builder.AppendLine("\tnode[shape=box];");
			var dict = new Dictionary<Unit.UnitBase, int>();
			

			dict[renderer] = 0;
			PrintLabel(renderer, builder, dict);
			PrintTopologyUnit(renderer, builder, dict);

			builder.AppendLine("}");
			File.WriteAllText(fileName, builder.ToString());
		}

		static void PrintTopologyUnit(Unit.UnitBase unit, StringBuilder builder, Dictionary<Unit.UnitBase, int> dict)
		{
			foreach (var item in unit.In.Select(x => x.FromUnit))
			{
				if (!dict.ContainsKey(item))
				{
					dict[item] = dict.Count;
					PrintLabel(item, builder, dict);
					PrintTopologyUnit(item, builder, dict);
				}
				builder.AppendFormat("\t{0}_{2} -> {1}_{3};\n", item, unit, dict[item], dict[unit]);
			}
		}

		static void PrintLabel(Unit.UnitBase unit, StringBuilder builder, Dictionary<Unit.UnitBase, int> dict)
		{
			builder.AppendFormat("\t{0}_{1}[label=\"{0}\"];\n", unit, dict[unit]);
		}
	}
}
