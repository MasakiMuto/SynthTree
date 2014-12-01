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
			Out.Value = In1.Value + In2.Value;
		}
	}
}
