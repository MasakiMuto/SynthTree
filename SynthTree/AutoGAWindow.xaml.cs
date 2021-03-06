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
using System.Windows.Shapes;

namespace SynthTree
{
	/// <summary>
	/// AutoGAWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class AutoGAWindow : Window
	{
		AutoGA ga;
		List<Tuple<double, double, double>> scoreHistory;

		Tree.RootNode initialTree;

		public AutoGAWindow()
		{
			InitializeComponent();
		}

		System.Diagnostics.Stopwatch stopwatch;
		System.Threading.CancellationTokenSource canceller;

		async void Run()
		{
			
			if (ga != null && ga.IsRunning)
			{
				return;
			}
			Cursor = Cursors.Wait;
			scoreHistory = new List<Tuple<double, double, double>>();
			ShowScorePlot();
			ga = new AutoGA(targetFile, int.Parse(poolSize.Text));
			canceller = new System.Threading.CancellationTokenSource();
			
			ga.Cancell = canceller.Token;
			ga.OnUpdate = Update;
			ga.Initial = initialTree;
			var gen = int.Parse(maxGeneration.Text);
			stopwatch = new System.Diagnostics.Stopwatch();
			stopwatch.Start();
			await Task.Run(() => { ga.Init(); ga.Run(gen); }, canceller.Token);
			stopwatch.Stop();
			canceller.Dispose();
			canceller = null;
			ga.SaveResult(System.IO.Path.ChangeExtension(targetFile, GAResult.Extension));
			//ga.BestElite.Tree.Serialize(System.IO.Path.ChangeExtension(targetFile, "bin"));
			generation.Content = "complete";
			Cursor = null;
			MainWindow.Instance.SetInitial(ga.BestElite.Tree);
		}

		OxyPlot.Series.LineSeries series;

		void ShowScorePlot()
		{
			var model = new OxyPlot.PlotModel();
			model.Axes.Add(new OxyPlot.Axes.LinearAxis()
			{
				Title = "score",
				Position = OxyPlot.Axes.AxisPosition.Left,
			});
			model.Axes.Add(new OxyPlot.Axes.LinearAxis()
			{
				Title = "generation",
				Position = OxyPlot.Axes.AxisPosition.Bottom,
				Maximum = int.Parse(maxGeneration.Text)
			});
			series = new OxyPlot.Series.LineSeries()
			{
				Title = "best"
			};
			
			model.Series.Add(series);
			//ser = new OxyPlot.Series.LineSeries()
			//{
			//	Title = "average"
			//};
			//ser.Points.AddRange(scoreHistory.Select((x, i) => new OxyPlot.DataPoint(i, x.Item2)));
			//model.Series.Add(ser);
			//ser = new OxyPlot.Series.LineSeries()
			//{
			//	Title = "distribution"
			//};
			//ser.Points.AddRange(scoreHistory.Select((x, i) => new OxyPlot.DataPoint(i, x.Item3)));
			//model.Series.Add(ser);
			plot.Model = model;
			plot.InvalidatePlot();

		}

		void UpdatePlot()
		{
			series.Points.Clear();
			series.Points.AddRange(scoreHistory.Select((x, i) => new OxyPlot.DataPoint(i, x.Item1)));
			plot.InvalidatePlot();
		}

		void Update()
		{
			var avg = ga.Scores.Average();
			var dist = Math.Sqrt(ga.Scores.Sum(x => (x - avg) * (x - avg)) / ga.Scores.Length);
			scoreHistory.Add(Tuple.Create(ga.BestScore, avg, dist));
			App.Current.Dispatcher.Invoke(() =>
			{
				generation.Content = ga.Generation;
				value.Content = ga.BestScore;
				failCount.Content = ga.FailCount;
				time.Content = stopwatch.Elapsed.ToString();
				UpdatePlot();
			});
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Run();
		}

		private void EndButtonClick(object sender, RoutedEventArgs e)
		{
			if (canceller != null)
			{
				canceller.Cancel();
			}
		}

		private void LoadTreeButtonClick(object sender, RoutedEventArgs e)
		{
			var dialog = new Microsoft.Win32.OpenFileDialog()
			{
				Filter = "tree binary | *.bin"
			
			};
			if (dialog.ShowDialog() ?? false)
			{
				initialTree = Tree.RootNode.Deserialize(dialog.FileName);
			}
		}

		string targetFile = "result.wav";

		private void SelectTargetButtonClick(object sender, RoutedEventArgs e)
		{
			var dialog = new Microsoft.Win32.OpenFileDialog(){
				Filter = "wave file | *.wav",
			};
			if (dialog.ShowDialog() ?? false)
			{
				targetFile = dialog.FileName;
				AnalyzeWindow.Window.OpenFile(dialog.FileName);
			}
		}
	}
}
