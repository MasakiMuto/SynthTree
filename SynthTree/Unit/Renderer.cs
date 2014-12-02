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

		public double RequireValue()
		{
			Require();
			return In[0].Value;
		}
	}
}
