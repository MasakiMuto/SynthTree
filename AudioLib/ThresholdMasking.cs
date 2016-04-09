using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;

namespace AudioLib
{
	public class ThresholdMasking
	{
		readonly int WindowSize = 512;


		Complex[] samples;
		readonly int SampleRate;

		public ThresholdMasking(Complex[] samples, int samplerate)
		{
			this.samples = samples;
			WindowSize = samples.Length * 2;
			SampleRate = samplerate;

		}

		public double[] Calc()
		{
			var t = Enumerable.Range(1, CriticalBands.Length - 1).Select(x => T(x)).ToArray();
			var unit = SampleRate / (double)samples.Length;
			var r = new double[samples.Length];
			int c = 1;
			for (int i = 0; i < r.Length; i++)
			{
				r[i] = t[c];
				if (CriticalBands[c+1] < i * unit && c + 1 < t.Length)
				{
					c++;
				}
			}
			return r;
		}


		#region ThresholdMasking

		static readonly double[] CriticalBands = new double[]{
			0, 20,  100,  200,  300,  400,  510,  630,  770,
			920, 1080, 1270, 1480, 1720, 2000, 2320, 2700,
			3150, 3700, 4400, 5300, 6400, 7700, 9500,
			12000, 15500	
		};


		/// <summary>
		/// z
		/// </summary>
		/// <param name="f"></param>
		/// <returns></returns>
		double BarkScale(double f)
		{
			return 13.0 * Math.Atan(0.76 * f / 1000) + 3.5 * Math.Atan(Math.Pow(f / 7500, 2));
		}

		/// <summary>
		/// B(z)
		/// </summary>
		/// <param name="z"></param>
		/// <returns></returns>
		double BasilarMembrane(double z)
		{
			return 15.91 + 7.5 * (z + 0.474) - 17.5 * Math.Sqrt(1 + Math.Pow(z + 0.474, 2));
		}

		double Sm(int z)
		{
			return Spz(z) * BasilarMembrane(z);
		}

		double SFM()
		{
			int Zt = CriticalBands.Length;
			var spz = Enumerable.Range(1, Zt - 1).Select(i => Spz(i));
			var up = spz.Aggregate(1.0, (s, x) => s * x);
			var down = spz.Sum() / Zt;
			return 10.0 * Math.Log10(Math.Pow(up / down, 1.0 / Zt));
		}

		double O(double z)
		{
			const double Max = -60.0;
			var alpha = Math.Min(SFM() / Max, 1);
			return alpha * (14.5 + z) + (1 - alpha) * 5.5;
		}

		double Tnorm(int z)
		{
			var raw = Math.Pow(10, Sm(z) - O(z) / 10.0);
			var t = GetBinRange(z);
			var p = (t.Item2 - t.Item1 + 1);
			if (p == 0)
			{
				return 0;
			}
			return raw / p;
		}

		double T(int z)
		{
			return Tnorm(z);
			//var power = 
		}

		int GetNearBinIndex(double f)
		{
			var unit = SampleRate / (WindowSize / 2.0);
			return (int)(f / unit);
		}

		Tuple<int, int> GetBinRange(int z)
		{
			var lbz = CriticalBands[z - 1];
			var hbz = CriticalBands[z];
			var l = GetNearBinIndex(lbz);
			var h = GetNearBinIndex(hbz) - 1;
			return Tuple.Create(l, h);
		}

		double Spz(int z)
		{
			var t = GetBinRange(z);
			int l = t.Item1;
			int h = t.Item2;
			return Enumerable.Range(l, h - l + 1)
				.Select(i => samples[i])
				.Sum(c => c.Magnitude);
		}



		#endregion


	}
}
