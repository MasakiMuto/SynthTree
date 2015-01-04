using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace AudioLib
{
	using Data = Single;

	public class Recorder : IDisposable
    {
		public int SelectedDevice { get;set; }

		WaveIn waveIn;

		readonly int SampleRate = 8000;
		readonly int Channels = 1;
		WaveFileWriter writer;

		public bool IsRecording { get { return writer != null; } }

		public Recorder()
		{
			waveIn = new WaveIn();
			waveIn.DataAvailable += waveIn_DataAvailable;
			waveIn.WaveFormat = new WaveFormat(SampleRate, Channels);
			waveIn.StartRecording();
		}

		void waveIn_DataAvailable(object sender, WaveInEventArgs e)
		{
			var data = Enumerable.Range(0, e.BytesRecorded / 2)
				.Select(i => (short)((e.Buffer[i*2 + 1] << 8) | e.Buffer[i*2]))
				.Select(x => x / 32768.0f);
			if (ProcessData != null)
			{
				ProcessData(data.ToArray());
			}
			
		}

		public void BeginRecord()
		{
			if (writer != null)
			{
				throw new InvalidOperationException("recording is already started");
			}
			writer = new WaveFileWriter(System.IO.Path.GetTempFileName(), WaveFormat.CreateIeeeFloatWaveFormat(SampleRate, Channels));
			ProcessData += RecordData;
		}

		public void EndRecord(string fn)
		{
			if (writer == null)
			{
				throw new InvalidOperationException("recording is not started");
			}
			ProcessData -= RecordData;
			var origin = writer.Filename;
			writer.Close();
			writer.Dispose();
			writer = null;
			System.IO.File.Copy(origin, fn, true);
		}

		void RecordData(Data[] data)
		{
			writer.WriteSamples(data, 0, data.Length);
		}

		public IEnumerable<string> GetDeviceNames()
		{
			var count = WaveIn.DeviceCount;
			return Enumerable.Range(0, count)
				.Select(x => WaveIn.GetCapabilities(x).ProductName);
		}

		public event Action<Data[]> ProcessData;

		public void Dispose()
		{
			if (writer != null)
			{
				writer.Dispose();
				writer = null;
			}
			if (waveIn != null)
			{
				waveIn.Dispose();
				waveIn = null;
			}
		}
    }
}
