using System;
using System.IO;
using DeltaEngine.Extensions;
using DeltaEngine.Multimedia.MusicStreams;
using Microsoft.Xna.Framework.Audio;

namespace DeltaEngine.Multimedia.Xna
{
	internal class XnaBufferedMusic : IDisposable
	{
		public XnaBufferedMusic(Stream stream)
		{
			musicStream = new MusicStreamFactory().Load(stream);
			var channels = musicStream.Channels == 2 ? AudioChannels.Stereo : AudioChannels.Mono;
			source = new DynamicSoundEffectInstance(musicStream.Samplerate, channels);
		}

		private BaseMusicStream musicStream;
		private DynamicSoundEffectInstance source;

		public void SetVolume(float value)
		{
			source.Volume = value;
		}

		public void Play()
		{
			musicStream.Rewind();
			for (int index = 0; index < NumberOfBuffers; index++)
				if (!Stream())
					break;
			source.Play();
			playStartTime = DateTime.Now;
		}

		private const int NumberOfBuffers = 2;
		private DateTime playStartTime;

		private bool Stream()
		{
			try
			{
				return TryStream();
			}
			catch (Exception)
			{
				return false;
			}
		}

		private bool TryStream()
		{
			var buffer = new byte[BufferSize];
			int bytesRead = musicStream.Read(buffer, BufferSize);
			if (bytesRead == 0)
				return false;
			source.SubmitBuffer(buffer, 0, bytesRead);
			return true;
		}

		public void Stop()
		{
			source.Stop();
		}

		public bool Run()
		{
			if (UpdateBuffersAndCheckFinished())
				return true;
			if (!IsPlaying)
				source.Play();
			return false;
		}

		private bool UpdateBuffersAndCheckFinished()
		{
			if (source.PendingBufferCount == 0)
				if (!Stream())
					return true;
			return false;
		}

		protected const int BufferSize = 1024 * 16;

		public void Dispose()
		{
			if (source != null)
				source.Dispose();
			source = null;
			musicStream = null;
		}

		public bool IsPlaying
		{
			get { return source.State != SoundState.Stopped; }
		}

		public float DurationInSeconds
		{
			get { return musicStream.LengthInSeconds; }
		}

		public float PositionInSeconds
		{
			get
			{
				var seconds = (float)DateTime.Now.Subtract(playStartTime).TotalSeconds;
				return seconds.Clamp(0f, DurationInSeconds).Round(2);
			}
		}
	}
}
