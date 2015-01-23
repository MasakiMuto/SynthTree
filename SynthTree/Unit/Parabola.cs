using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree.Unit
{
	public class Parabola : Unit1In1Out
	{
		public double A, B, C;

		public override void Update()
		{
			base.Update();
			var x = In[0].Value;
			Out[0].Value = A * x * x + B * x + C;
		}
	}
}
