using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree.Unit
{
	public class Renderer : UnitRender
	{
		public override void Update()
		{
			base.Update();
		}

		public double RequireValue(long t)
		{
			Require(t);
			return In[0].Value;
		}

		public override string ToString()
		{
			return "Renderer";
		}
	}
}
