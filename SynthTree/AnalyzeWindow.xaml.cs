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
		public static AnalyzeWindow Window { get; private set; }

		public AnalyzeWindow()
		{
			InitializeComponent();
			Window = this;
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

		public AudioLib.Analyzer Analyzer { get; private set; }

		public async void OpenFile(string fn)
		{
			this.Cursor = Cursors.Wait;
			Analyzer = new AudioLib.Analyzer(fn);
			Settings.Instance.SamplingFreq = Analyzer.SampleRate;
			//analyzer.Dft();
			//analyzer.CalcSpectrogram();
			await Task.Run(() => Analyzer.CalcPower());
			await Task.Run(() => Analyzer.CalcPitch());
			this.Cursor = null;
			//SetSpector(analyzer.Freq.Select((x, i) => new OxyPlot.DataPoint(analyzer.FreqPerIndex * i, x)));

			plot.Model = SetSpectrogram(Analyzer.Pitch.Select((x, i) => new OxyPlot.DataPoint(i / (double)Settings.Instance.SamplingFreq, x)));
			plot2.Model = SetPowergram(Analyzer.PowerTime.Select((x, i) => new OxyPlot.DataPoint(i / (double)Settings.Instance.SamplingFreq, x)));
			plot2.InvalidatePlot();
			plot.InvalidatePlot();

			DevelopManager.SetSource(Analyzer.Pitch.Select(x=> x / (FileUtil.SampleRate / 2)).ToArray(), Analyzer.PowerTime);
		}

		OxyPlot.PlotModel SetSpector(IEnumerable<OxyPlot.DataPoint> data)
		{
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
			var series = new OxyPlot.Series.LineSeries();
			series.Points.AddRange(data);
			model.Series.Add(series);
			return model;
		}

		OxyPlot.PlotModel SetPowergram(IEnumerable<OxyPlot.DataPoint> data)
		{
			var model = new OxyPlot.PlotModel()
			{
				Title = "velocity"
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
				Title = "pitch"
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

		private void Window_Closed(object sender, EventArgs e)
		{
			App.Current.Shutdown();
		}

		
		
	}
}
