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
	/// AutoGAWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class AutoGAWindow : Window
	{
		AutoGA ga;

		public AutoGAWindow()
		{
			InitializeComponent();
			
		}

		async void Run()
		{
			ga = new AutoGA("result.wav", int.Parse(poolSize.Text));
			ga.OnUpdate = Update;
			
			ga.Init();
			var gen = int.Parse(maxGeneration.Text);
			await Task.Run(()=>ga.Run(gen));
			ga.BestElite.Tree.Serialize("ga.bin");
		}

		void Update()
		{
			App.Current.Dispatcher.Invoke(() =>
			{
				generation.Content = ga.Generation;
				value.Content = ga.BestScore;
				failCount.Content = ga.FailCount;
			});
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Run();
		}
	}
}
