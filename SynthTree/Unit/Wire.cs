using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree.Unit
{
	public class Wire : Unit1In1Out
	{
		public override void Update()
		{
			base.Update();
			Out[0].Value = In[0].Value;
		}

		public override string ToString()
		{
			return "Wire";
		}
	}
}
