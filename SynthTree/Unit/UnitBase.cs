using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree.Unit
{
	public abstract class UnitBase
	{
		protected long Count { get; private set; }
		public virtual void Update()
		{
			Count++;
		}

	}

	//public abstract class Unit1In1Out : UnitBase
	//{
	//	public Connection In;
	//	public Connection Out;
	//}

	public abstract class UnitSource : UnitBase
	{
		public Connection Out;
	}

	public abstract class UnitRender : UnitBase
	{
		public Connection In;
	}

	public abstract class Unit2In1Out : UnitBase
	{
		public Connection In1, In2;
		public Connection Out;
	}

	public abstract class Unit1In2Out : UnitBase
	{
		public Connection In;
		public Connection Out1, Out2;
	}

	public class Connection
	{
		public double Value;
		public UnitBase FromUnit;
		public UnitBase ToUnit;

		public Connection(UnitBase from, UnitBase to)
		{
			FromUnit = from;
			ToUnit = to;
		}
	}
}
