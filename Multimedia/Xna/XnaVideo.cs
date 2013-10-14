using System;
using System.Diagnostics;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Graphics.Xna;
using DeltaEngine.Rendering2D;
using DeltaEngine.ScreenSpaces;
using XnaMedia = Microsoft.Xna.Framework.Media;

namespace DeltaEngine.Multimedia.Xna
{
	/// <summary>
	/// Native Xna implementation for video playback.
	/// </summary>
	public class XnaVideo : Video
	{
		public XnaVideo(string filename, XnaMedia.VideoPlayer player, XnaDevice graphicsDevice,
			SoundDevice soundDevice)
			: base(filename, soundDevice)
		{
			this.player = player;
			this.graphicsDevice = graphicsDevice;
			image = new VideoImage(graphicsDevice);
		}

		private readonly XnaMedia.VideoPlayer player;
		private readonly XnaDevice graphicsDevice;
		private XnaMedia.Video video;
		private readonly VideoImage image;

		protected override void PlayNativeVideo(float volume)
		{
			positionInSeconds = 0f;
			player.Volume = volume;
			player.Play(video);
			surface = new Sprite(new Material(ContentLoader.Load<Shader>(Shader.Position2DUV), image),
				ScreenSpace.Current.Viewport);
		}

		private Sprite surface;
		private float positionInSeconds;

		protected override void StopNativeVideo()
		{
			if (surface != null)
				surface.IsActive = false;

			surface = null;
			player.Stop();
		}

		public override bool IsPlaying()
		{
			return player.State != XnaMedia.MediaState.Stopped && IsActiveVideo();
		}

		private bool IsActiveVideo()
		{
			return player.Video == video;
		}

		protected override void LoadData(Stream fileData)
		{
			try
			{
				video = graphicsDevice.NativeContent.Load<XnaMedia.Video>(Name);
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
				if (Debugger.IsAttached)
					throw new XnaVideoContentNotFound(Name, ex);
			}
		}

		private class XnaVideoContentNotFound : Exception
		{
			public XnaVideoContentNotFound(string contentName, Exception exception)
				: base(contentName, exception) {}
		}

		protected override void DisposeData()
		{
			base.DisposeData();
			video = null;
		}

		public override float DurationInSeconds
		{
			get { return (float)video.Duration.TotalSeconds; }
		}

		public override float PositionInSeconds
		{
			get { return positionInSeconds; }
		}

		public override void Update()
		{
			if (!IsActiveVideo())
				return;

			image.UpdateTexture(player.GetTexture());
			positionInSeconds = (float)player.PlayPosition.TotalSeconds;
		}
	}
}