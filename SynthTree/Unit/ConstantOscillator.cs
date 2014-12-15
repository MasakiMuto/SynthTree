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

		public override void Update()
		{
			base.Update();
			Out[0].Value = Constant;
			Out[1].Value = Math.Sin(In[0].Value + Phase);
		}

		public override string ToString()
		{
			return "ConstantOscillator";
		}
	}
}
