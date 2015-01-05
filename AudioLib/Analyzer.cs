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
		readonly int WindowSize = 1024;
		readonly float[] sound;
		readonly int SampleRate;
		public double[] Freq { get; private set; }
		public double FreqPerIndex { get { return SampleRate / (double)WindowSize; } }
		readonly int FreqPerSample = 10;//何サンプルごとにスペクトルを計算するか
		double[,] Spectrogram;

		public Analyzer(string fileName)
		{
			using (var file = new WaveFileReader(fileName))
			{
				sound = new float[file.SampleCount / file.WaveFormat.Channels];
				for (int i = 0; i < sound.Length; i++)
				{
					sound[i] = file.ReadNextSampleFrame()[0];
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

		void CalcSpectrogram()
		{
			var window = Window.Hamming(WindowSize);
			Spectrogram = new double[sound.Length / FreqPerSample, WindowSize / 2];
			for (int m = 0; m < Spectrogram.GetLength(0); m++)
			{
				var w = sound.Skip(m * FreqPerSample)
					.Take(WindowSize)
					.Select((x, i) => x * (float)window[i])
					.Select(x => new Complex(x, 0))
					.ToArray();
				Fourier.Radix2Forward(w, FourierOptions.Matlab);
				for (int i = 0; i < Spectrogram.GetLength(1); i++)
				{
					Spectrogram[m, i] = w[i].Magnitude;
				}
				//var w = sound.Select((x, j) => x * (float)window[j]).Select(x => new Complex(x, 0)).ToArray();
				//Fourier.Forward(w, FourierOptions.Matlab);
				//Freq = w.Take(w.Length / 2).Select(x => x.Magnitude).ToArray();

			}
		}

		void Dft()
		{
			var window = Window.Hamming(WindowSize);
			var w = sound.Select((x, j) => x * (float)window[j]).Select(x => new Complex(x, 0)).ToArray();
			Fourier.Forward(w, FourierOptions.Matlab);
			Freq = w.Take(w.Length / 2).Select(x => x.Magnitude).ToArray();
	

		}



	}
}
