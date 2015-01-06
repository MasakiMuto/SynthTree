using System;
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
	/// Interaction logic for AnalyzeWindow.xaml
	/// </summary>
	public partial class AnalyzeWindow : Window
	{
		public AnalyzeWindow()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new Microsoft.Win32.OpenFileDialog()
			{
				Filter = "SoundFile | *.wav",
				Multiselect = false,
			};
			if (dialog.ShowDialog() ?? false)
			{
				OpenFile(dialog.FileName);
			}
		}

		OxyPlot.Series.LineSeries series;

		void OpenFile(string fn)
		{
			var analyzer = new AudioLib.Analyzer(fn);
			analyzer.Dft();
			analyzer.CalcSpectrogram();
			analyzer.CalcPower();
			var model = new OxyPlot.PlotModel();
			model.Axes.Add(new OxyPlot.Axes.LinearAxis()
			{
				Position = OxyPlot.Axes.AxisPosition.Left,
				Title = "power"
			});
			model.Axes.Add(new OxyPlot.Axes.LogarithmicAxis()
			{
				Position = OxyPlot.Axes.AxisPosition.Bottom,
				Title = "Freq",
			});
			series = new OxyPlot.Series.LineSeries();
			series.Points.AddRange(analyzer.Freq.Select((x, i) => new OxyPlot.DataPoint(analyzer.FreqPerIndex * i, x)));
			model.Series.Add(series);
			plot.Model = model;
			plot.InvalidatePlot();

			plot.Model = SetSpectrogram(analyzer.FreqTime.Select((x, i) => new OxyPlot.DataPoint(i * analyzer.SecondPerIndex, x)));
			plot2.Model = SetPowergram(analyzer.PowerTime.Select((x, i) => new OxyPlot.DataPoint(i * analyzer.SecondPerIndex, x)));
			plot2.InvalidatePlot();
			plot.InvalidatePlot();
		}

		OxyPlot.PlotModel SetPowergram(IEnumerable<OxyPlot.DataPoint> data)
		{
			var model = new OxyPlot.PlotModel()
			{
				Title = "powergram"
			};
			model.Axes.Add(new OxyPlot.Axes.LinearAxis()
			{
				Position = OxyPlot.Axes.AxisPosition.Bottom,
				Title = "t"
			});
			model.Axes.Add(new OxyPlot.Axes.LinearAxis()
			{
				Title = "Power(dB)",
				Position = OxyPlot.Axes.AxisPosition.Left
			});
			var s = new OxyPlot.Series.LineSeries();
			s.Points.AddRange(data);
			model.Series.Add(s);
			return model;
		}

		OxyPlot.PlotModel SetSpectrogram(IEnumerable<OxyPlot.DataPoint> spec)
		{
			var model = new OxyPlot.PlotModel()
			{
				Title = "spectrogram/powergram"
			};
			model.Axes.Add(new OxyPlot.Axes.LinearAxis()
			{
				Position = OxyPlot.Axes.AxisPosition.Bottom,
				Title = "t"
			});
			model.Axes.Add(new OxyPlot.Axes.LogarithmicAxis()
			{
				Position = OxyPlot.Axes.AxisPosition.Left,
				Title = "freq",
				Minimum = 10,
			});
			var series = new OxyPlot.Series.LineSeries();
			series.Points.AddRange(spec);
			model.Series.Add(series);
			return model;
		}

		
		
	}
}
