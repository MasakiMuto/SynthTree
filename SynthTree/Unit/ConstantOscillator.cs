using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree.Unit
{
	public class ConstantOscillator : Unit1In2Out
	{
		public double Constant;
		public double Phase;

		double val;

		public override void Update()
		{
			base.Update();
			Out[0].Value = Constant;
			val += In[0].Value;
			Out[1].Value = Math.Sin(val + Phase * Math.PI * 2);
		}

		public override string ToString()
		{
			return "ConstantOscillator";
		}
	}
}
