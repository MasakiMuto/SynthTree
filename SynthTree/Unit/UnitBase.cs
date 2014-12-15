﻿using System;
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

		public long Count { get; private set; }
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

		/// <summary>
		/// あるユニットを別のものに置き換える
		/// </summary>
		/// <param name="old"></param>
		/// <param name="next"></param>
		public static void Reconnect(UnitBase old, UnitBase next)
		{
			for (int i = 0; i < old.In.Length; i++)
			{
				next.In[i] = old.In[i];
				next.In[i].ToUnit = next;
			}
			for (int i = 0; i < old.Out.Length; i++)
			{
				next.Out[i] = old.Out[i];
				next.Out[i].FromUnit = next;
			}
		}

		/// <summary>
		/// oldのInputの指定ノードをnextのInputの指定ノードに繋ぎ変え、oldのInputの指定ノードを新たにnewInputのOutputの指定ノードにつなぐ
		/// </summary>
		/// <param name="old"></param>
		/// <param name="oldIndex"></param>
		/// <param name="next"></param>
		/// <param name="nextIndex"></param>
		/// <param name="newInput"></param>
		/// <param name="newInputIndex"></param>
		public static void ReplaceInput(UnitBase old, int oldIndex, UnitBase next, int nextIndex, UnitBase newInput, int newInputIndex)
		{
			next.In[nextIndex] = old.In[oldIndex];
			next.In[nextIndex].ToUnit = next;
			Connect(newInput, newInputIndex, old, oldIndex);
		}

		public static void ReplaceOutput(UnitBase old, int oldIndex, UnitBase next, int nextIndex, UnitBase newOutput, int newOutputIndex)
		{
			next.Out[nextIndex] = old.Out[oldIndex];
			next.Out[nextIndex].FromUnit = next;
			Connect(old, oldIndex, newOutput, newOutputIndex);
		}

		protected void Require()
		{
			if (this.In.Length == 0)
			{
				Update();
				return;
			}
			else
			{
				foreach (var item in In)
				{
					item.FromUnit.Require();
				}
				Update();
			}
		}
		

	}

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

		public static Unit2In1Out CreateStub()
		{
			return new Adder();
		}
	}

	public abstract class Unit1In2Out : UnitBase
	{
		public Unit1In2Out()
		{
			In = new Connection[1];
			Out = new Connection[2];
		}

		public static Unit1In2Out CreateStub()
		{
			return new Splitter();
		}
	}

	public class UnitWire : UnitBase
	{
		public UnitWire()
		{
			In = new Connection[1];
			Out = new Connection[1];
		}

		public override void Update()
		{
			base.Update();
			Out[0].Value = In[0].Value;
		}
	}

	public class Connection
	{
		double val;
		public double Value
		{
			get { return val; }
			set
			{
				Debug.Assert(Count + 1 == FromUnit.Count);
				val = value;
				Count = FromUnit.Count;
			}
		}
		public UnitBase FromUnit;
		public UnitBase ToUnit;
		public long Count;

		public Connection(UnitBase from, UnitBase to)
		{
			FromUnit = from;
			ToUnit = to;
		}

	}

}
