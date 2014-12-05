using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree.Tree
{
	public class Modifier : TreeBase
	{
		enum ConnectionType
		{
			Series1A,
			Series2A,

		}

		ConnectionType type;
		Unit.Unit2In1Out target;


		public override void Process()
		{
			var s1 = new Unit.UnitStub2In1Out(0);
			var s2 = new Unit.UnitStub1In2Out(0);
			Unit.UnitBase.Connect(target.In[1].FromUnit, 0, s2, 0);
			Unit.UnitBase.Connect(s1, 0, target.Out[0].ToUnit, 0);
			Unit.UnitBase.Connect(s2, 0, target, 1);
			Unit.UnitBase.Connect(s2, 1, s1, 1);
			Unit.UnitBase.Connect(target, 0, s1, 0);

		}
	}

}
