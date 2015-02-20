using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using System.IO;

namespace ConsoleGA
{
	class Options
	{
		[Option('s', "size", DefaultValue = 50, HelpText = "GA Pool Size")]
		public int PoolSize { get; set; }

		[Option('g', "generation", DefaultValue = 100, HelpText = "GA Max Generation")]
		public int MaxGeneration { get; set; }

		[HelpOption]
		public string GetUsage()
		{
			return "help";
		}

		[ValueList(typeof(List<string>))]
		public List<string> Items { get; set; }

	}

	class Program
	{
		static void Main(string[] args)
		{
			var options = new Options();
			if (CommandLine.Parser.Default.ParseArguments(args, options))
			{
				foreach (var item in options.Items)
				{
					if (Directory.Exists(item))
					{
						foreach (var fn in Directory.EnumerateFiles(item, "*.wav"))
						{
							Process(fn, options);
						}
					}
					else
					{
						Process(item, options);
					}
				}
			}
		}

		static void Process(string file, Options options)
		{
			Console.Write("Start {0} ...", file);
			var ga = new SynthTree.AutoGA(file, options.PoolSize);
			ga.Init();
			ga.Run(options.MaxGeneration);
			var dir = Path.Combine(Path.GetDirectoryName(file), "result");
			Directory.CreateDirectory(dir);
			var result = Path.Combine(dir, Path.ChangeExtension(Path.GetFileName(file), SynthTree.GAResult.Extension));
			ga.SaveResult(result);
			Console.WriteLine("score {1}, Saved to {0}", result, ga.BestScore);
		}
	}
}
