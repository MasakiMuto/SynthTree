using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree.Unit
{
	public sealed class Adder : Unit2In1Out
	{
		public override void Update()
		{
			base.Update();
			Out[0].Value = In[0].Value + In[1].Value;
		}
	}
}
