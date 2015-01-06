using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using System.Numerics;

namespace AudioLib
{
	public class Analyzer
	{
		readonly int WindowSize = 512;
		readonly float[] sound;
		readonly int SampleRate;
		public double[] Freq { get; private set; }
		public double FreqPerIndex { get { return SampleRate / (double)WindowSize; } }
		public double SecondPerIndex { get { return 1.0 / (SampleRate / FreqPerSample); } }
		readonly int FreqPerSample = 10;//何サンプルごとにスペクトルを計算するか
		public double[,] Spectrogram;
		public double[] FreqTime;//時間ごと最大成分周波数
		public double[] PowerTime;//時間ごとボリューム(デシベル)
		readonly long ActualDataLength;

		public Analyzer(string fileName)
		{
			using (var file = new WaveFileReader(fileName))
			{
				ActualDataLength = file.SampleCount / file.WaveFormat.Channels;
				sound = new float[ActualDataLength + WindowSize];//-WindowSize/2 ~ 0と Length ~ Length + WindowSize/2はダミー
				for (int i = 0; i < ActualDataLength; i++)
				{
					sound[i + WindowSize / 2] = file.ReadNextSampleFrame()[0];
				}
				SampleRate = file.WaveFormat.SampleRate;
			}
			//Dft();
		}

		public Analyzer(float[] data, int sampleRate)
		{
			sound = data.Clone() as float[];
			SampleRate = sampleRate;
			//Dft();
		}

		public void CalcSpectrogram()
		{
			var window = Window.Hamming(WindowSize);
			Spectrogram = new double[ActualDataLength / FreqPerSample, WindowSize / 2];
			FreqTime = new double[Spectrogram.GetLength(0)];
			PowerTime = new double[Spectrogram.GetLength(0)];
			for (int m = 0; m < Spectrogram.GetLength(0); m++)
			{
				var w = sound.Skip(m * FreqPerSample)
					.Take(WindowSize)
					.Select((x, i) => x * (float)window[i])
					.Select(x => new Complex(x, 0))
					.ToArray();
				//if (w.Length < WindowSize)
				//{
				//	w = w.Concat(Enumerable.Repeat(new Complex(0, 0), WindowSize - w.Length)).ToArray();
				//}
				Fourier.Radix2Forward(w, FourierOptions.Matlab);
				double tmp = -1;
				for (int i = 0; i < Spectrogram.GetLength(1); i++)
				{
					var mag = w[i].Magnitude;
					Spectrogram[m, i] = mag;
					if (mag > tmp)
					{
						FreqTime[m] = i * this.FreqPerIndex;
						tmp = mag;
					}
					PowerTime[m] += mag;
				}
				PowerTime[m] = Math.Log10(PowerTime[m]);
				//var w = sound.Select((x, j) => x * (float)window[j]).Select(x => new Complex(x, 0)).ToArray();
				//Fourier.Forward(w, FourierOptions.Matlab);
				//Freq = w.Take(w.Length / 2).Select(x => x.Magnitude).ToArray();

			}
		}

		public void Dft()
		{
			var window = Window.Hamming(WindowSize);
			var w = sound
				.Take(WindowSize)
				.Select((x, j) => x * (float)window[j])
				.Select(x => new Complex(x, 0)).ToArray();
			Fourier.Forward(w, FourierOptions.Matlab);
			Freq = w.Take(w.Length / 2).Select(x => x.Magnitude).ToArray();
	

		}

		public void CalcPower()
		{
			const int Length = 512;
			PowerTime = new double[ActualDataLength / FreqPerSample];
			for (int i = 0; i < PowerTime.Length; i++)
			{

				PowerTime[i] = sound.Skip(i * FreqPerSample).Take(Length)
					.Select(x => x * x)
					.Sum();
				PowerTime[i] = Math.Sqrt(PowerTime[i] / Length);
			}
		}


	}
}
