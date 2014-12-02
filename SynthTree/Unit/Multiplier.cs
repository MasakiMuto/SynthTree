using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree.Unit
{
	public class Multiplier : Unit2In1Out
	{
		public override void Update()
		{
			base.Update();
			Out.Value = In1.Value * In2.Value;
		}
	}
}
