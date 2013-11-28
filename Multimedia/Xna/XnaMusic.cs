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
		public XnaMusic(string filename, XnaSoundDevice device, ContentManager contentManager)
			: base(filename, device)
		{
			this.contentManager = contentManager;
		}

		private readonly ContentManager contentManager;

		protected override void SetPlayingVolume(float value)
		{
			if (IsXnaContentValid())
				MediaPlayer.Volume = value;
			else
				bufferedMusic.SetVolume(value);
		}

		protected override void PlayNativeMusic()
		{
			try
			{
				if (IsXnaContentValid())
				{
					positionInSeconds = 0f;
					MediaPlayer.Play(music);
				}
				else
					bufferedMusic.Play();
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
				if (Debugger.IsAttached)
					throw new XnaMusicContentNotFound(Name, ex);
			}
		}

		private Song music;
		private float positionInSeconds;

		private bool IsXnaContentValid()
		{
			if (checkedBefore)
				return cachedXnaContentAvailable;
			checkedBefore = true;
			cachedXnaContentAvailable = contentManager != null &&
				File.Exists(Path.Combine(contentManager.RootDirectory, Name + ".xnb"));
			return cachedXnaContentAvailable;
		}

		private bool checkedBefore;
		private bool cachedXnaContentAvailable;

		protected override void StopNativeMusic()
		{
			if (IsXnaContentValid())
				MediaPlayer.Stop();
			else
				bufferedMusic.Stop();
		}

		public override bool IsPlaying()
		{
			return IsXnaContentValid()
				? (MediaPlayer.State != MediaState.Stopped && IsActiveMusic()) : bufferedMusic.IsPlaying;
		}

		private bool IsActiveMusic()
		{
			return MediaPlayer.Queue.Count > 0 && MediaPlayer.Queue.ActiveSong == music;
		}

		public override void Run()
		{
			if (IsXnaContentValid())
			{
				positionInSeconds = (float)MediaPlayer.PlayPosition.TotalSeconds;
				if (!IsPlaying())
					HandleStreamFinished();
			}
			else
			{
				if (bufferedMusic.Run())
					HandleStreamFinished();
			}
		}

		protected override void LoadData(Stream fileData)
		{
			try
			{
				if (IsXnaContentValid())
					LoadXnaContent();
				else
					LoadNormalContent(fileData);
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
				if (Debugger.IsAttached)
					throw new XnaMusicContentNotFound(Name, ex);
			}
		}

		private void LoadXnaContent()
		{
			music = contentManager.Load<Song>(Name);
		}

		private void LoadNormalContent(Stream fileData)
		{
			var stream = new MemoryStream();
			fileData.CopyTo(stream);
			stream.Seek(0, SeekOrigin.Begin);
			bufferedMusic = new XnaBufferedMusic(stream);
		}

		private XnaBufferedMusic bufferedMusic;

		public class XnaMusicContentNotFound : Exception
		{
			public XnaMusicContentNotFound(string contentName, Exception exception)
				: base(contentName, exception) {}
		}

		protected override void DisposeData()
		{
			if (IsXnaContentValid())
			{
				if (music == null)
					return;
				base.DisposeData();
				music.Dispose();
				music = null;
			}
			else
			{
				base.DisposeData();
				bufferedMusic.Dispose();
			}
		}

		public override float DurationInSeconds
		{
			get
			{
				return IsXnaContentValid()
					? (float)music.Duration.TotalSeconds : bufferedMusic.DurationInSeconds;
			}
		}

		public override float PositionInSeconds
		{
			get { return IsXnaContentValid() ? positionInSeconds : bufferedMusic.PositionInSeconds; }
		}
	}
}