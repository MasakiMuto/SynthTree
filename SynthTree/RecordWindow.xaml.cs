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
using OxyPlot.Wpf;

namespace SynthTree
{
	/// <summary>
	/// RecordWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class RecordWindow : Window
	{
		public RecordWindow(AudioLib.Recorder recorder)
		{
			InitializeComponent();
			plot.Model = CreateModel();
			this.recorder = recorder;
			recorder.ProcessData += ProcessData;
		}

		void ProcessData(float[] data)
		{
			series.Points.Clear();
			series.Points.AddRange(data.Select((x, i)=>new OxyPlot.DataPoint(i, x)));
			plot.InvalidatePlot(true);
		}

		AudioLib.Recorder recorder;

		OxyPlot.Series.LineSeries series;

		

		OxyPlot.PlotModel CreateModel()
		{
			var model = new OxyPlot.PlotModel();
			model.Axes.Add(new OxyPlot.Axes.LinearAxis()
			{
				Maximum = 1f,
				Minimum = -1f,
				Position = OxyPlot.Axes.AxisPosition.Left
			});
			model.Axes.Add(new OxyPlot.Axes.LinearAxis()
			{
				Position = OxyPlot.Axes.AxisPosition.Bottom
			});
			series = new OxyPlot.Series.LineSeries()
			{

			};
			model.Series.Add(series);
			return model;
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			recorder.ProcessData -= ProcessData;
		}
	}
}
