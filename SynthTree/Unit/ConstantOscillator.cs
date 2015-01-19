using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree.Unit
{
	public class ConstantOscillator : Unit1In2Out
	{
		enum WaveForm
		{
			Sine,
			Rectangle,
			Sawtooth,
			Noise
		}

		public double Constant;
		public double Phase;
		public double WaveFormValue
		{
			set
			{
				const int Types = 4;
				if (value < 1.0 / Types)
				{
					waveForm = ConstantOscillator.WaveForm.Sine;
				}
				else if (value < 2.0 / Types)
				{
					waveForm = ConstantOscillator.WaveForm.Rectangle;
				}
				else if (value < 3.0 / Types)
				{
					waveForm = ConstantOscillator.WaveForm.Sawtooth;
				}
				else
				{
					waveForm = ConstantOscillator.WaveForm.Noise;
				}
			}
		}

		static double[] NoiseBuffer;

		WaveForm waveForm;

		double val;

		static ConstantOscillator()
		{
			var rand = new Random(0);
			NoiseBuffer = Enumerable.Range(0, 64).Select(x => rand.NextDouble() * 2 - 1).ToArray();
		}

		public override void Update()
		{
			base.Update();
			Out[0].Value = Constant;
			val += In[0].Value;
			Out[1].Value = CalcWave(val * MaxFreq + Phase * Math.PI * 2);
		}

		double CalcWave(double phase)
		{
			double n = phase % (Math.PI * 2);
			if (n < 0)
			{
				n += Math.PI * 2;
			}
			switch (waveForm)
			{
				case WaveForm.Sine:
					return Math.Sin(phase);
				case WaveForm.Rectangle:
					return n < Math.PI ? 0.5 : -0.5;
				case WaveForm.Sawtooth:
					return -1 + n / (Math.PI * 2);
				case WaveForm.Noise:
					var index = (int)phase % NoiseBuffer.Length;
					if (index < 0)
					{
						index += NoiseBuffer.Length;
					}
					return NoiseBuffer[index];
				default:
					throw new Exception();
			}
		}

		public override string ToString()
		{
			return "ConstantOscillator";
		}
	}
}
