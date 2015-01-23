using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree.Unit
{
	public class StepDelay : Unit1In1Out
	{
		double state;

		public override void Init()
		{
			base.Init();
			state = 0;
		}

		public override void Update()
		{
			base.Update();
			Out[0].Value = state;
			state = In[0].Value;
		}

		public override string ToString()
		{
			return "StepDelay";
		}
	}
}
