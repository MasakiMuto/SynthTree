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

namespace SynthTree
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var fn = "test.wav";
			var file = new FileUtil(fn);
			var src = new Unit.WaveSource();
			var render = new Unit.Renderer();
			Unit.UnitBase.Connect(src, 0, render, 0);
			file.Write(Enumerable.Range(0, 44100).Select(x =>
				{
					src.Update();
					return (float)render.In[0].Value;
				}));
			file.Dispose();
			new System.Media.SoundPlayer(fn).Play();
		}
	}
}
