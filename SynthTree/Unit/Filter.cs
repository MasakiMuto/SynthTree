using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree.Unit
{
	class Filter : Unit1In1Out
	{
		public double LpCutoff, LpResonance, LpSweep, HpCutoff, HpSweep;

		double lpCutoff, lpResonance, lpSweep, hpCutoff, hpSweep;

		double lp, ldp, hp;

		public override void Init()
		{
			base.Init();
			lp = 0;
			ldp = 0;
			hp = 0;
			LpCutoff = Math.Abs(LpCutoff);
			LpResonance = Math.Abs(LpResonance);
			HpCutoff = Math.Abs(HpCutoff);
			lpCutoff = Math.Pow(LpCutoff, 3) * .1;
			lpSweep = 1.0 + LpSweep * 0.0001;
			lpResonance = 5.0 / (1.0 + Math.Pow(LpResonance, 2.0) * 20) * (0.01 + lpCutoff);
			lpResonance = Math.Min(0.8, lpResonance);

			hpCutoff = Math.Pow(HpCutoff, 2) * 0.1;
			hpSweep = 1.0 + HpSweep * 0.0003;

		}

		public override void Update()
		{
			base.Update();
			var pp = lp;
			lpCutoff *= lpSweep;
			lpCutoff = Math.Min(Math.Max(0.0, lpCutoff), 0.1);
			hpCutoff *= hpSweep;
			hpCutoff = Math.Min(Math.Max(0.00001, hpCutoff), 0.1);

			ldp += (In[0].Value - lp) * lpCutoff;
			ldp -= ldp * lpResonance;
			lp += ldp;
			hp += lp - pp;
			hp -= hp * hpCutoff;
			Out[0].Value = hp;
		}

		public override string ToString()
		{
			return "Filter";
		}
	}
}
