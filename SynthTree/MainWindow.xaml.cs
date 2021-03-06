﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Media;

namespace SynthTree
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		public static MainWindow Instance { get; private set; }

		public GAManager Manager { get; private set; }
		SoundItemControl[] soundControls;

		Tree.RootNode tree;
		Individual individual;

		AudioLib.Recorder recorder;
		AnalyzeWindow analyzer;

		string defaultFileName;

		public MainWindow()
		{
			Instance = this;
			InitializeComponent();
			Manager = new GAManager();
			soundControls = Enumerable.Range(0, 9)
				.Select(i =>
				{
					var item = new SoundItemControl(i, this);
					item.SetValue(Grid.RowProperty, i / 3);
					item.SetValue(Grid.ColumnProperty, i % 3);
					return item;
				})
				.ToArray();
			foreach (var item in soundControls)
			{
				grid.Children.Add(item);
			}
			recorder = new AudioLib.Recorder();
			analyzer = new AnalyzeWindow();
			analyzer.Show();
			new AutoGAWindow().Show();

			var args = Environment.GetCommandLineArgs();
			if (args.Length == 2)
			{
				LoadGaResult(args[1]);
			}
		}

		void GenerateClick(object sender, RoutedEventArgs e)
		{
			do
			{
				tree = DevelopManager.CreateInitialTree();
				individual = new Individual(tree, true);
			} while (!individual.IsValidWaveform());
			
			individual.Play();
		}


		

		private void StartButtonClick(object sender, RoutedEventArgs e)
		{
			if (tree == null)
			{
				return;
			}
			Cursor = Cursors.Wait;
			Manager.Start(tree);
			UpdateControls();
			Cursor = null;
			PlayAllClick(null, null);
		}

		private void SaveButtonClick(object sender, RoutedEventArgs e)
		{
			var target = soundControls.FirstOrDefault(x => x.IsChecked);
			if (target != null)
			{
				var dialog = new Microsoft.Win32.SaveFileDialog()
				{
					DefaultExt = ".wav",
					Filter = "wave file | *.wav",
					AddExtension = true,
					FileName = defaultFileName,
				};
				if (dialog.ShowDialog() ?? false)
				{
					var ind = Manager[target.Index];
					ind.SaveSound();
					System.IO.File.Copy(ind.Sound, dialog.FileName);
					ind.Tree.Serialize(System.IO.Path.ChangeExtension(dialog.FileName, ".bin"));
				}
			
			}
		}

		private void PlayAllClick(object sender, RoutedEventArgs e)
		{
			Task.Factory.StartNew(() =>
			{
				for (int i = 0; i < soundControls.Length; i++)
				{
					soundControls[i].Play();
					
				}
			});
			
		}

		private void NextButtonClick(object sender, RoutedEventArgs e)
		{
			Cursor = Cursors.Wait;
			Manager.Update(GetSelectedIndex());
			Cursor = null;
			UpdateControls();
			PlayAllClick(null, null);
		}

		void UpdateControls()
		{
			generationLabel.Content = Manager.Pool.Generation;
		}

		IEnumerable<int> GetSelectedIndex()
		{
			return soundControls.Where(x => x.IsChecked).Select(x => x.Index);
		}

		void RecordButtonClick(object sender, RoutedEventArgs e)
		{
			if (recorder.IsRecording)
			{
				recorder.EndRecord("record.wav");
			}
			else
			{
				new RecordWindow(recorder).Show();
				recorder.BeginRecord();
			}
		}

		void LoadClick(object sender, RoutedEventArgs e)
		{
			var dialog = new Microsoft.Win32.OpenFileDialog()
			{
				AddExtension = true,
				DefaultExt = "bin",
				Filter = "files|*.bin;*.result|tree binary|*.bin|GA result|*.result"
			};
			if (dialog.ShowDialog() ?? false)
			{
				LoadGaResult(dialog.FileName);
			}
			
		}

		void LoadGaResult(string fileName)
		{
			defaultFileName = System.IO.Path.ChangeExtension(fileName, ".wav");
			if (GAResult.IsGAResultFile(fileName))
			{
				var res = GAResult.Load(fileName);
				analyzer.OpenFile(res.TargetFileName);
				SetInitial(res.Tree);
				
			}
			else
			{
				SetInitial(Tree.RootNode.Deserialize(fileName));
			}
				
		}

		public void SetInitial(Tree.RootNode t)
		{
			this.tree = t;
			individual = new Individual(tree, true);
		}

		private void PreviewButtonClick(object sender, RoutedEventArgs e)
		{
			if (individual != null)
			{
				individual.Play();
				//Util.Visualizer.ShowTopology(individual.Topology);
				//Util.Visualizer.ShowTree(individual.Tree);
			}
		}

		private void EnterThresholdButtonClick(object sender, RoutedEventArgs e)
		{
			if (Manager == null || !Manager.Ready)
			{
				return;
			}
			int val;
			if (!int.TryParse(thresholdBox.Text, out val))
			{
				val = 100;
				thresholdBox.Text = val.ToString();
			}
			int max;
			if (int.TryParse(thresholdMax.Text, out max))
			{
				Manager.Pool.ThresholdMax = max;
			}
			Manager.Pool.ThresholdMin = val;
			if (Manager.Pool.ThresholdMax <= Manager.Pool.ThresholdMin)
			{
				Manager.Pool.ThresholdMax = double.MaxValue;
				System.Windows.MessageBox.Show("類似度閾値の最小値が最大値を上回っています");
			}
		}

		private void CompareButtonClick(object sender, RoutedEventArgs e)
		{
			var target = GetSelectedIndex().ToArray();
			if (target.Length != 2)
			{
				return;
			}
			if (Manager == null || !Manager.Ready)
			{
				return;
			}
			var val = Manager[target[0]].CompareTo(Manager[target[1]]);
			similarityLabel.Content = "score:" + (int)val;
		}

		private void Undo(object sender, RoutedEventArgs e)
		{
			Manager.Pool.Undo();
			UpdateControls();
		}

		private void Redo(object sender, RoutedEventArgs e)
		{
			Manager.Pool.Redo();
			UpdateControls();
		}


	}
}
