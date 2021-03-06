﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTree
{
	public class Individual
	{
		public readonly Tree.RootNode Tree;
		public readonly Unit.Renderer Topology;
		public string Sound { get; private set; }
		public readonly AudioLib.Analyzer Analyzer;
		public readonly float[] Data;

		public Individual(Tree.RootNode tree, bool useMask)
		{
			Tree = tree;
			Topology = DevelopManager.DevelopTopology(tree);
			Data = Enumerable.Range(0, DevelopManager.TableLength).Select(x => (float)Topology.RequireValue(x)).ToArray();
			Normalize();
			Analyzer = new AudioLib.Analyzer(Data, (int)FileUtil.SampleRate);
			Analyzer.CalcSpectrogram(useMask);
			
		}

		public void SaveSound()
		{
			Sound = System.IO.Path.GetTempFileName();
			using (var f = new FileUtil(Sound))
			{
				f.Write(Data);
			}
		}

		void Normalize()
		{
			float max = 0;
			float sum = 0;
			foreach (var item in Data)
			{
				if (Math.Abs(item) > max)
				{
					max = Math.Abs(item);
				}
				sum += item;
			}
			if (max == 0)
			{
				return;
			}
			float avg = sum / Data.Length;
			for (int i = 0; i < Data.Length; i++)
			{
				Data[i] = (Data[i] - avg) / max;
				if(float.IsNaN(Data[i]))
				{
					//System.Diagnostics.Debugger.Break();
					Data[i] = 0;
				}
			}
		}

		public double CompareTo(Individual another)
		{
			return CompareTo(another.Analyzer);
		}

		public double CompareTo(AudioLib.Analyzer another)
		{

			return CompareThresholdMasking(another);
			//return CompareSpectrogram(another);
		}

		double CompareThresholdMasking(AudioLib.Analyzer target)
		{
			double s = 0;
			for (int i = 0; i < Analyzer.Spectrogram.GetLength(0); i++)
			{
				for (int j = 0; j < Analyzer.Spectrogram.GetLength(1); j++)
				{
					var m = target.ThresholdMask[i][j];
					var o = Analyzer.Spectrogram[i, j];
					var t = target.Spectrogram[i, j];
					if (t >= m)
					{
						s += (o - t) * (o - t);
					}
					if (o >= t)
					{
						s += (o - m) * (o - m);
					}
				}
			}
			s /= Analyzer.Spectrogram.GetLength(0);
			return s;
		}

		double CompareSpectrogram(AudioLib.Analyzer another)
		{
			double s = 0;
			for (int i = 0; i < Analyzer.Spectrogram.GetLength(0); i++)
			{
				for (int j = 0; j < Analyzer.Spectrogram.GetLength(1); j++)
				{
					s += Math.Pow(Analyzer.Spectrogram[i, j] - another.Spectrogram[i, j], 2);
				}
			}
			s /= Analyzer.Spectrogram.GetLength(0);//frame length
			return s;
		}

		public void Play()
		{
			if (Sound == null)
			{
				SaveSound();
			}
			using (var audio = new System.Media.SoundPlayer(Sound))
			{
				audio.PlaySync();
			}
		}

		public bool IsValidWaveform()
		{
			Analyzer.CalcPitch();
			return Analyzer.Pitch.Any(x => x != 0);
		}

	}
}
