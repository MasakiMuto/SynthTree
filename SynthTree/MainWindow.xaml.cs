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

namespace SynthTree
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		public GAManager Manager { get; private set; }
		SoundItemControl[] soundControls;

		Tree.RootNode tree;

		AudioLib.Recorder recorder;
		AnalyzeWindow analyzer;

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
			recorder = new AudioLib.Recorder();
			analyzer = new AnalyzeWindow();
			analyzer.Show();
		}

		void GenerateClick(object sender, RoutedEventArgs e)
		{
			tree = DevelopManager.CreateInitialTree();
			//tree = Tree.RootNode.Deserialize("test.txt");
			//Util.Visualizer.ShowTree(tree);
			new ItemPool.ItemSet(tree).Play();
		}


		

		private void StartButtonClick(object sender, RoutedEventArgs e)
		{
			Cursor = Cursors.Wait;
			Manager.Start(tree);

			Cursor = null;
			//var dev = new DevelopManager();
			//using(var f =new FileUtil("test1.wav"))
			//{
			//	f.Write(Enumerable.Range(0, 44100).Select(x => (float)dev.render.RequireValue()));
			//}
			//using (var s = new SoundPlayer("test1.wav"))
			//{
			//	s.Play();
			//}
		}

		private void SaveButtonClick(object sender, RoutedEventArgs e)
		{
			var target = soundControls.FirstOrDefault(x => x.IsChecked);
			if (target != null)
			{
				Manager.Save(target.Index);
				Manager[target.Index].Tree.Serialize("tree.bin");
			}
		}

		private async void PlayAllClick(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < soundControls.Length; i++)
			{
				await soundControls[i].Play();
				//await Task.Delay(300);
			}
		}

		private void NextButtonClick(object sender, RoutedEventArgs e)
		{
			Cursor = Cursors.Wait;
			Manager.Update(soundControls.Where(x => x.IsChecked).Select(x=>x.Index));
			Cursor = null;
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
				Filter = "tree binary|*.bin"
			};
			if (dialog.ShowDialog() ?? false)
			{
				tree = Tree.RootNode.Deserialize(dialog.FileName);
			}
			
		}


	}
}
