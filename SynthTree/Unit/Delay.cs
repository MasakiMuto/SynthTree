using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree.Unit
{
	public class Delay : Unit1In1Out
	{
		public double Offset, Sweep;

		double[] buffer;
		double phase, delta;

		public override void Init()
		{
			base.Init();
			buffer = new double[1024];
			phase = Offset * Offset * 1020;
			if (Offset < 0)
			{
				phase *= -1;
			}
			delta = Sweep * Sweep;
			if (Sweep < 0)
			{
				delta *= -1;
			}
		}

		public override void Update()
		{
			int c = (int)Count & 1023;
			base.Update();
			phase += delta;
			buffer[c] = In[0].Value;
			Out[0].Value = buffer[(c - Math.Min(1023, Math.Abs((int)phase)) + 1024) & 1023];
		}

		public override string ToString()
		{
			return "Delay";
		}
	}
}
