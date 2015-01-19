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

		public AutoGAWindow()
		{
			InitializeComponent();
			
		}

		async void Run()
		{
			
			if (ga != null && ga.IsRunning)
			{
				return;
			}
			Cursor = Cursors.Wait;
			scoreHistory = new List<Tuple<double, double, double>>();
			ga = new AutoGA("result.wav", int.Parse(poolSize.Text));
			ga.OnUpdate = Update;
			
			ga.Init();
			var gen = int.Parse(maxGeneration.Text);
			var stopwatch = new System.Diagnostics.Stopwatch();
			stopwatch.Start();
			await Task.Run(()=>ga.Run(gen));
			stopwatch.Stop();
			time.Content = stopwatch.Elapsed.ToString();
			ga.BestElite.Tree.Serialize("ga.bin");
			generation.Content = "complete";
			ShowScorePlot();
			Cursor = null;
		}

		void ShowScorePlot()
		{
			var model = new OxyPlot.PlotModel();
			model.Axes.Add(new OxyPlot.Axes.LogarithmicAxis()
			{
				Title = "score",
				Position = OxyPlot.Axes.AxisPosition.Left,
				Minimum = 1,
			});
			model.Axes.Add(new OxyPlot.Axes.LinearAxis()
			{
				Title = "generation",
				Position = OxyPlot.Axes.AxisPosition.Bottom
			});
			var ser = new OxyPlot.Series.LineSeries()
			{
				Title = "best"
			};
			ser.Points.AddRange(scoreHistory.Select((x, i) => new OxyPlot.DataPoint(i, x.Item1)));
			model.Series.Add(ser);
			ser = new OxyPlot.Series.LineSeries()
			{
				Title = "average"
			};
			ser.Points.AddRange(scoreHistory.Select((x, i) => new OxyPlot.DataPoint(i, x.Item2)));
			model.Series.Add(ser);
			ser = new OxyPlot.Series.LineSeries()
			{
				Title = "distribution"
			};
			ser.Points.AddRange(scoreHistory.Select((x, i) => new OxyPlot.DataPoint(i, x.Item3)));
			model.Series.Add(ser);
			plot.Model = model;
			plot.InvalidatePlot();

		}

		void Update()
		{
			App.Current.Dispatcher.Invoke(() =>
			{
				generation.Content = ga.Generation;
				value.Content = ga.BestScore;
				failCount.Content = ga.FailCount;
				
			});
			var avg = ga.Scores.Average();
			var dist = Math.Sqrt(ga.Scores.Sum(x => (x - avg) * (x - avg)) / ga.Scores.Length);
			scoreHistory.Add(Tuple.Create(ga.BestScore, avg, dist));
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Run();
		}
	}
}