﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SynthTree.Unit;

namespace SynthTree.Tree
{
	public enum ModifierType
	{
		SeriesA1,
		ParallelA1,
		SeriesB1,
		ParallelB1,
	}

	public class Modifier : TreeBase
	{
		readonly ModifierType type;

		public Modifier(ModifierType type)
		{
			this.type = type;
		}

		public override void Process()
		{
			System.Diagnostics.Debug.Assert((Target is Unit2In1Out && (type == ModifierType.ParallelA1 || type == ModifierType.SeriesA1))
				|| (Target is Unit1In2Out && (type == ModifierType.ParallelB1 || type == ModifierType.SeriesB1)));
			switch (type)
			{
				case ModifierType.SeriesA1:
					SeriesA1();
					break;
				case ModifierType.ParallelA1:
					break;
				case ModifierType.SeriesB1:
					break;
				case ModifierType.ParallelB1:
					break;
				default:
					break;
			}

		}

		void SeriesA1()
		{
			var a = Unit2In1Out.CreateStub();
			var b = Unit1In2Out.CreateStub();
			Unit.UnitBase.Connect(b, 1, a, 1);
			UnitBase.ReplaceInput(Target, 1, b, 0, b, 0);
			UnitBase.ReplaceOutput(Target, 0, a, 0, a, 0);
		}

		void ParallelA1()
		{
			var a1 = Unit2In1Out.CreateStub();
			var a2 = Unit2In1Out.CreateStub();
			var b1 = Unit1In2Out.CreateStub();
			var b2 = Unit1In2Out.CreateStub();

			UnitBase.ReplaceInput(Target, 0, b1, 0, a1, 0);
			UnitBase.ReplaceInput(Target, 1, b2, 0, a2, 0);
			UnitBase.Connect(b1, 0, a1, 0);
			UnitBase.Connect(b1, 1, a2, 0);
			UnitBase.Connect(b2, 0, a1, 1);
			UnitBase.Connect(b2, 1, a2, 1);
		}

		void SeriesB1()
		{
			var a = Unit2In1Out.CreateStub();
			var b = Unit1In2Out.CreateStub();
			UnitBase.ReplaceOutput(Target, 0, b, 0, b, 0);
			UnitBase.ReplaceOutput(Target, 1, a, 0, a, 1);
			UnitBase.Connect(b, 1, a, 0);
		}

		void ParallelB1()
		{
			var a0 = Unit2In1Out.CreateStub();
			var a1 = Unit2In1Out.CreateStub();
			var b0 = Unit1In2Out.CreateStub();
			var b1 = Unit1In2Out.CreateStub();

			UnitBase.ReplaceOutput(Target, 0, a0, 0, b0, 0);
			UnitBase.ReplaceOutput(Target, 1, a1, 0, b1, 0);
			UnitBase.Connect(b0, 0, a0, 0);
			UnitBase.Connect(b1, 1, a1, 1);
			UnitBase.Connect(b0, 1, a1, 0);
			UnitBase.Connect(b1, 0, a0, 1);
		}
	}

}
