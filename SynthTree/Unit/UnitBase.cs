using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SynthTree.Unit
{
	public abstract class UnitBase
	{
		public Connection[] In;
		public Connection[] Out;

		protected long Count { get; private set; }
		public virtual void Update()
		{
			Count++;
		}

		public static void Connect(UnitBase from, int fromIndex, UnitBase to, int toIndex)
		{
			var con = new Connection(from, to);
			Debug.Assert(fromIndex == 0 || (fromIndex == 1 && from is Unit1In2Out));
			Debug.Assert(!(from is UnitRender));
			Debug.Assert(!(to is UnitSource));
			Debug.Assert(toIndex == 0 || (toIndex == 1 && to is Unit2In1Out));

			from.Out[fromIndex] = con;
			to.In[toIndex] = con;

		}
		

	}

	//public abstract class Unit1In1Out : UnitBase
	//{
	//	public Connection In;
	//	public Connection Out;
	//}

	public abstract class UnitSource : UnitBase
	{
		public UnitSource()
		{
			In = new Connection[0];
			Out = new Connection[1];
		}
	}

	public abstract class UnitRender : UnitBase
	{
		public UnitRender()
		{
			In = new Connection[1];
			Out = new Connection[0];
		}
	}

	public abstract class Unit2In1Out : UnitBase
	{

		public Unit2In1Out()
		{
			In = new Connection[2];
			Out = new Connection[1];
		}
	}

	public abstract class Unit1In2Out : UnitBase
	{
		public Unit1In2Out()
		{
			In = new Connection[1];
			Out = new Connection[2];
		}
	}

	public class UnitStub2In1Out : Unit2In1Out
	{
		public override void Update()
		{
			throw new InvalidOperationException();
		}
	}

	public class UnitStub1In2Out : Unit1In2Out
	{
		public override void Update()
		{
			throw new InvalidOperationException();
		}
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
