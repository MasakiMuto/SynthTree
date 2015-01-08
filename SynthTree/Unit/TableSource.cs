using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree.Unit
{
	public class TableSource : UnitSource
	{
		readonly double[] Table;

		public TableSource(double[] table)
		{
			this.Table = table;
		}

		public override void Update()
		{
			base.Update();
			Out[0].Value = Table[Count -1];
			
		}

		public override string ToString()
		{
			return "TableSource";
		}
	}
}
