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
		readonly int NoOverlap = 256;
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
				sound = new float[ActualDataLength];
				for (int i = 0; i < ActualDataLength; i++)
				{
					sound[i] = file.ReadNextSampleFrame()[0];
				}
				SampleRate = file.WaveFormat.SampleRate;
			}
		}

		public Analyzer(float[] data, int sampleRate)
		{
			sound = data;
			ActualDataLength = sound.Length;
			SampleRate = sampleRate;
		}

		IEnumerable<double> GetWindowedData(int from, int length)
		{
			var window = Window.Hamming(length);
			return Enumerable.Range(from, length)
				.Select(x => sound.ElementAtOrDefault(x))
				.Select((x, j) => x * window[j]);
		}

		public void CalcSpectrogram()
		{
			int FrameOverlap = WindowSize / 2;
			int NoOverlap = WindowSize - FrameOverlap;
			Spectrogram = new double[ActualDataLength / NoOverlap, WindowSize / 2];
			Enumerable.Range(0, Spectrogram.GetLength(0))
				.AsParallel()
				.ForAll(i =>
				{
					var w = GetWindowedData(i * NoOverlap - WindowSize / 2, WindowSize)
						.Select(x=>new Complex(x, 0))
						.ToArray();
					Fourier.Radix2Forward(w, FourierOptions.Matlab);
					for (int j = 0; j < w.Length / 2; j++)
					{
						Spectrogram[i, j] = w[j].Magnitude;
					}				
				});
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

		void CalcInner(ref double[] target, Func<int, double> func)
		{
			var tmp = Enumerable.Range(0, (int)ActualDataLength / NoOverlap)
				.Select(i => func(i))
				.ToArray();
			target = new double[NoOverlap * tmp.Length];
			for (int i = 0; i < tmp.Length - 1; i++)
			{
				var step = (tmp[i + 1] - tmp[i]) / NoOverlap;
				for (int j = 0; j < NoOverlap; j++)
				{
					target[i * NoOverlap + j] = tmp[i] + step * j;
				}
			}
		}

		/// <summary>
		/// 長さActualDataLengthの配列
		/// </summary>
		public void CalcPower()
		{
			CalcInner(ref PowerTime, CalcPower);
		}

		IEnumerable<float> CreateFrame(int from)
		{
			return Enumerable.Range(from * NoOverlap - NoOverlap, WindowSize)
				.SkipWhile(x => x < 0)
				.TakeWhile(x => x < ActualDataLength)
				.Select(x => sound[x]);
		}

		double CalcPower(int from)
		{
			int c = 0;
			double s = 0;
			foreach (var i in CreateFrame(from))
			{
				s += i * i; ;
				c++;
			}
			if (c == 0)
			{
				return 0;
			}
			else
			{
				return s / c;
			}
		}

		const int AutoCorrelationLength = 512;

		/// <summary>
		/// 
		/// </summary>
		public void CalcPitch()
		{
			CalcInner(ref FreqTime, CalcPitch);
		}

		double CalcPitch(int from)
		{
			var f = CreateFrame(from).ToArray();


			var r = new double[AutoCorrelationLength];
			double tmp = double.NegativeInfinity;
			int max = 0;
			for (int i = 0; i < AutoCorrelationLength; i++)
			{
				r[i] = Enumerable.Range(0, AutoCorrelationLength - 1)
					.Select(t => sound[t + from] * sound[i + t + from])
					.Sum();
				r[i] /= r[0];
				if (i > 0 && r[i] > tmp)
				{
					max = i;
					tmp = r[i];
				}
			}
			return SampleRate / (double)max;
	
		}


	}
}
