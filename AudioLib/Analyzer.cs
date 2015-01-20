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

		IEnumerable<float> CreateFrame(int from, bool window)
		{
			var w = Window.Hamming(WindowSize);
			var head = from * NoOverlap - WindowSize / 2;
			return Enumerable.Range(head, WindowSize)
				.SkipWhile(x => x < 0)
				.TakeWhile(x => x < ActualDataLength)
				.Select(x => sound[x] * (window ? (float) w[x - head] : 1f));
		}

		double CalcPower(int from)
		{
			int c = 0;
			double s = 0;
			foreach (var i in CreateFrame(from, false))
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

		/// <summary>
		/// 基本周波数を求める
		/// </summary>
		public void CalcPitch()
		{
			CalcInner(ref FreqTime, CalcPitch);
		}

		double CalcPitch(int from)
		{
			const double K = 0.8;
			var n = CalcNormalizedSquareDifferenceFunction(CreateFrame(from, false).ToArray());

			List<Tuple<double, int>> peaks = new List<Tuple<double, int>>();
			bool stat = false;
			double m = -1;
			int t = -1;
		
			for (int i = 1; i < n.Length; i++)
			{
				if (n[i] >= 0 && n[i - 1] < 0)
				{
					stat = true;
				}
				if (n[i] < 0 && n[i - 1] >= 0 && stat)
				{
					stat = false;
					peaks.Add(Tuple.Create(m, t));
				}
				if (stat && n[i] > m)
				{
					m = n[i];
					t = i;
				}
			}
			if (!peaks.Any())
			{
				return 0;
			}

			var thr = peaks.Max(x => x.Item1) * K;
			int time = peaks.First(x => x.Item1 >= thr).Item2;
			return SampleRate / time;
		}

		double[] CalcNormalizedSquareDifferenceFunction(float[] f)
		{
			double[] n = new double[f.Length];
			double m, r;
			for (int tau = 0; tau < f.Length; tau++)
			{
				m = 0;
				r = 0;
				for (int j = 0; j < f.Length - tau - 1; j++)
				{
					m += f[j] * f[j] + f[j + tau] * f[j + tau];
					r += f[j] * f[j + tau];
				}
				if(m != 0)
				{ 
					n[tau] = 2 * r / m;
				}
				else
				{
					n[tau] = 0;
				}
			}
			return n;
		}


	}
}
