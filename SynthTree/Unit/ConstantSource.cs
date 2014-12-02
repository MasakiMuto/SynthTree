using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree.Unit
{
	public class ConstantSource : UnitSource
	{
		public double Value;

		public override void Update()
		{
			base.Update();
			Out[0].Value = Value;
		}
	}
}
