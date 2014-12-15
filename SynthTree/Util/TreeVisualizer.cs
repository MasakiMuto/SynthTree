using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace SynthTree.Util
{
	public static class TreeVisualizer
	{
		public static void Show(Tree.RootNode root)
		{
			var scriptName = "hoge.dot";
			var fileName = "hoge.png";
			CreateScript(root, scriptName);
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
			Process.Start(fileName);
			
			
		}

		static void CreateScript(Tree.RootNode root, string fileName)
		{
			var str = new StringBuilder();
			str.AppendFormat("digraph {0} {{ \n", Path.GetFileNameWithoutExtension(fileName));
			foreach (var item in root.ToBreadthFirstList())
			{
				PrintItems(item, str);
			}
			str.AppendLine("}");
			File.WriteAllText(fileName, str.ToString());
			
		}

		static void PrintItems(Tree.TreeBase node, StringBuilder builder)
		{
			foreach (var item in node.Children)
			{
				builder.AppendFormat("\t{0}_{2} -> {1}_{3};\n", node, item, node.Index, item.Index);
			}
		}
	}
}
