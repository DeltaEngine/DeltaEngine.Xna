using System;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using Microsoft.Xna.Framework.Graphics;

namespace DeltaEngine.Graphics.Xna
{
	[IgnoreForResolver]
	public class VideoImage : Image
	{
		public VideoImage(XnaDevice graphicsDevice)
			: base(new ImageCreationData(new Size(4, 4)))
		{
			nativeDevice = graphicsDevice.NativeDevice;
			if (nativeDevice == null || graphicsDevice.NativeContent == null)
				throw new UnableToContinueWithoutXnaGraphicsDevice();
			NativeTexture = new Texture2D(nativeDevice, (int)PixelSize.Width, (int)PixelSize.Height);
		}

		private readonly GraphicsDevice nativeDevice;

		private class UnableToContinueWithoutXnaGraphicsDevice : Exception {}

		public void UpdateTexture(Texture2D nativeTexture)
		{
			NativeTexture = nativeTexture;
		}

		public Texture2D NativeTexture { get; protected set; }

		protected override void SetSamplerStateAndTryToLoadImage(Stream fileData)
		{
			SetSamplerState();
		}

		protected override void LoadImage(Stream fileData) {}
		public override void Fill(Color[] colors) {}
		public override void Fill(byte[] rgbaColors) {}

		protected override void SetSamplerState()
		{
			nativeDevice.Textures[0] = NativeTexture;
			nativeDevice.SamplerStates[0] = DisableLinearFiltering
				? SamplerState.PointClamp : SamplerState.LinearClamp;
		}

		protected override void DisposeData()
		{
			if (NativeTexture != null)
				NativeTexture.Dispose();
			NativeTexture = null;
		}
	}
}