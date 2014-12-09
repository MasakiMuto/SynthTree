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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Media;

namespace IECSound
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		public GAManager Manager { get; private set; }
		SoundItemControl[] soundControls;

		public MainWindow()
		{
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
		}

		

		private void StartButtonClick(object sender, RoutedEventArgs e)
		{
			
		}

		private void SaveButtonClick(object sender, RoutedEventArgs e)
		{
			var target = soundControls.FirstOrDefault(x => x.IsChecked);
			if (target != null)
			{

			}
		}

		private async void PlayAllClick(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < soundControls.Length; i++)
			{
				await soundControls[i].Play();
				await Task.Delay(300);
			}
		}

		private void NextButtonClick(object sender, RoutedEventArgs e)
		{
			//Manager.Update(soundControls.Where(x => x.IsChecked).Select(x=>x.Index));
		}

	}
}
