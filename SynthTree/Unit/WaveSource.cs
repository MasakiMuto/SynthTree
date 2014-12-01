using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree.Unit
{
	public enum WaveForm
	{
		Sin,

	}

	public class WaveSource : UnitSource
	{
		public double Amp;
		public long Freq;
		WaveForm form;

		public WaveSource()
		{
			form = WaveForm.Sin;
			Amp = 1;
			Freq = 10000;
		}

		public override void Update()
		{
			base.Update();
			Out.Value = Math.Sin(CalcPhase()) * Amp;
		}

		double CalcPhase()
		{
			return Count * Math.PI * 2 / FileUtil.SampleRate * Freq;
		}
	}
}
