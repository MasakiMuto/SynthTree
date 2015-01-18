using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SynthTree
{
	/// <summary>
	/// ItemControl.xaml の相互作用ロジック
	/// </summary>
	public partial class SoundItemControl : UserControl
	{
		public int Index { get; private set; }
		readonly MainWindow window;
		public bool IsChecked { get { return check.IsChecked.GetValueOrDefault(false); } }

		Individual Item { get { return window.Manager[Index]; } }

		public SoundItemControl(int index, MainWindow window)
		{
			Index = index;
			this.window = window;
			InitializeComponent();
		}

		async void PlayClick(object sender, RoutedEventArgs e)
		{
			await Play();
		}

		void VisualClick(object sneder, RoutedEventArgs e)
		{
			if (Item != null)
			{
				Util.Visualizer.ShowTopology(Item.Topology);
			}
		}

		void TreeClick(object sender, RoutedEventArgs e)
		{
			if (Item != null)
			{
				Util.Visualizer.ShowTree(Item.Tree);
			}
		}

		public async Task Play()
		{
			if (Item == null)
			{
				return;
			}
			Activate();
			Item.Play();
			await Task.Delay(500);
			Deactivate();
		}

		public void Activate()
		{
			Background = Brushes.Red;
			InvalidateVisual();
		}

		public void Deactivate()
		{
			Background = Brushes.Transparent;
		}
	}
}
