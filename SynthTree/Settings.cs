using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SynthTree
{
	[Serializable]
	public class Settings
	{
		public static Settings Instance { get; private set; }

		const string FileName = "setting.xml";

		public int SamplingFreq { get; set; }

		public Settings()
		{
			SamplingFreq = 8000;
		}

		static Settings()
		{
			try
			{
				Instance = Deserialize();
			}
			catch (Exception)
			{
				Instance = new Settings();
				Instance.Serialize();
			}
		}

		public void Serialize()
		{
			var f = new XmlSerializer(typeof(Settings));
			using (var file = new FileStream(FileName, FileMode.Create, FileAccess.Write))
			{
				f.Serialize(file, this);
			}
		}

		static Settings Deserialize()
		{
			var f = new XmlSerializer(typeof(Settings));
			using (var file = new FileStream(FileName, FileMode.Open, FileAccess.Read))
			{
				return f.Deserialize(file) as Settings;
			}
		}

	}
}
