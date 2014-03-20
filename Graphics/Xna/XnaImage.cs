using System;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using Microsoft.Xna.Framework.Graphics;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace DeltaEngine.Graphics.Xna
{
	/// <summary>
	/// Type of Image used for Xna-Textures. Provides methods for loading and displaying that are
	/// native to Xna and not useful for other frameworks.
	/// </summary>
	public class XnaImage : Image
	{
		public XnaImage(string contentName, XnaDevice device)
			: base(contentName)
		{
			nativeDevice = device.NativeDevice;
			if (nativeDevice == null || device.NativeContent == null)
				throw new UnableToContinueWithoutXnaGraphicsDevice();
		}

		private XnaImage(ImageCreationData data, XnaDevice device)
			: base(data)
		{
			nativeDevice = device.NativeDevice;
			if (nativeDevice == null || device.NativeContent == null)
				throw new UnableToContinueWithoutXnaGraphicsDevice();
		}

		private readonly GraphicsDevice nativeDevice;

		private class UnableToContinueWithoutXnaGraphicsDevice : Exception {}

		protected XnaImage(XnaDevice device, Texture2D nativeTexture)
			: base("<NativeImage>")
		{
			nativeDevice = device.NativeDevice;
			NativeTexture = nativeTexture;
		}

		protected override void SetSamplerStateAndTryToLoadImage(Stream fileData)
		{
			TryLoadImage(fileData);
		}

		protected override void TryLoadImage(Stream fileData)
		{
			NativeTexture = Texture2D.FromStream(nativeDevice, fileData);
			var textureSize = new Size(NativeTexture.Width, NativeTexture.Height);
			CompareActualSizeMetadataSize(textureSize);
		}

		public Texture2D NativeTexture { get; protected set; }

		protected override void DisposeData()
		{
			if (NativeTexture != null)
				NativeTexture.Dispose();
		}

		public override void FillRgbaData(byte[] rgbaColors)
		{
			if (PixelSize.Width * PixelSize.Height * 4 != rgbaColors.Length)
				throw new InvalidNumberOfBytes(PixelSize);
			if (NativeTexture == null)
				NativeTexture = new Texture2D(nativeDevice, (int)PixelSize.Width, (int)PixelSize.Height);
			NativeTexture.SetData(rgbaColors);
		}

		protected override void SetSamplerState() {}
	}
}