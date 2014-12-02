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
			Out1.Value = Constant;
			Out2.Value = Math.Sin(In.Value + Phase);
		}
	}
}
