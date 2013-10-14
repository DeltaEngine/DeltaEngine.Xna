using System;
using System.IO;
using DeltaEngine.Core;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;

namespace DeltaEngine.Multimedia.Xna
{
	/// <summary>
	/// Native Xna implementation for music playback.
	/// </summary>
	public class XnaMusic : Music
	{
		public XnaMusic(string filename, XnaSoundDevice device, ContentManager contentManager,
			Settings settings)
			: base(filename, device, settings)
		{
			this.contentManager = contentManager;
		}

		private readonly ContentManager contentManager;

		protected override void PlayNativeMusic(float volume)
		{
			positionInSeconds = 0f;
			MediaPlayer.Volume = volume;
			MediaPlayer.Play(music);
		}

		private Song music;
		private float positionInSeconds;

		protected override void StopNativeMusic()
		{
			MediaPlayer.Stop();
		}
		
		public override bool IsPlaying()
		{
			return MediaPlayer.State != MediaState.Stopped && IsActiveMusic();
		}

		private bool IsActiveMusic()
		{
			return MediaPlayer.Queue.Count > 0 && MediaPlayer.Queue.ActiveSong == music;
		}

		public override void Run()
		{
			positionInSeconds = (float)MediaPlayer.PlayPosition.TotalSeconds;
			if (!IsPlaying())
				HandleStreamFinished();
		}

		protected override void LoadData(Stream fileData)
		{
			try
			{
				music = contentManager.Load<Song>(Name);
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
				if (Debugger.IsAttached)
					throw new XnaMusicContentNotFound(Name, ex);
			}
		}

		public class XnaMusicContentNotFound : Exception
		{
			public XnaMusicContentNotFound(string contentName, Exception exception)
				: base(contentName, exception) {}
		}

		protected override void DisposeData()
		{
			if (music == null)
				return;

			base.DisposeData();
			music.Dispose();
			music = null;
		}

		public override float DurationInSeconds
		{
			get { return (float)music.Duration.TotalSeconds; }
		}

		public override float PositionInSeconds
		{
			get { return positionInSeconds; }
		}
	}
}