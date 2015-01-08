using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace SynthTree
{
	class FileUtil : IDisposable
	{
		const uint ChunkSize = 16;
		const ushort CompressionCode = 1;
		const ushort Channels = 1;
		public const uint SampleRate = 8000;
		const ushort Bit = 16;
		const uint Bps = SampleRate * Bit / 8;
		const ushort BlockAlign = Bit / 8;

		BinaryWriter writer;
		long size;
		long sampleCount;
		float sample;
		int acc;

		public FileUtil(string path)
		{
			writer = new BinaryWriter(File.OpenWrite(path));
			WriteHeader();

		}

		public void Write(IEnumerable<float> data)
		{
			foreach (var item in data)
			{
				WriteValue(item);
			}		
		}

		void WriteValue(float val)
		{
			//val *= 4f;
			if (val > 1f)
			{
				val = 1f;
			}
			else if (val < -1f)
			{
				val = -1f;
			}
			sample = val;
			if (Bit == 16)
			{
				short isample = (short)(sample * 32760);
				writer.Write(isample);
			}
			else if(Bit == 8)
			{
				byte isample = (byte)(sample * 127 + 128);
				writer.Write(isample);
			}
			else
			{
				throw new NotImplementedException();
			}
			
			sampleCount++;
		}

		void WriteString(string s)
		{
			writer.Write(s.ToArray(), 0, s.Length);
		}

		public void Dispose()
		{
			if (writer == null)
			{
				return;
			}
			WriteFooter();
			writer.Close();
			writer = null;
			GC.SuppressFinalize(this);
		}

		~FileUtil()
		{
			Dispose();
		}

		

		void WriteHeader()
		{
			WriteString("RIFF");
			writer.Write((uint)0);
			WriteString("WAVE");

			WriteString("fmt ");
			writer.Write(ChunkSize);
			writer.Write(CompressionCode);
			writer.Write(Channels);
			writer.Write(SampleRate);
			writer.Write(Bps);
			writer.Write(BlockAlign);
			writer.Write(Bit);

			WriteString("data");
			writer.Flush();
			size = writer.BaseStream.Position;
			writer.Write((uint)0);

		}



		void WriteFooter()
		{
			writer.BaseStream.Seek(4, SeekOrigin.Begin);
			uint w = (uint)(size - 4 + sampleCount * Bit / 8);
			writer.Write(w);
			writer.BaseStream.Seek((int)size, SeekOrigin.Begin);
			w = (uint)(sampleCount * Bit / 8);
			writer.Write(w);
		}
	}
}
