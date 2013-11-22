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

		protected override void LoadImage(Stream fileData)
		{
			NativeTexture = Texture2D.FromStream(nativeDevice, fileData);
			//Disabled for M5 release due image server conversion not being enabled right now
			//if (HasAlpha && NativeTexture.Format != SurfaceFormat.Color)
			//	Logger.Warning("Image '" + Name +
			//		"' is supposed to have alpha pixels, but the image pixel format is not using alpha.");
			//else if (!HasAlpha && NativeTexture.Format == SurfaceFormat.Color)
			//	Logger.Warning("Image '" + Name +
			//		"' is supposed to have no alpha pixels, but the image pixel format is using alpha.");
			var textureSize = new Size(NativeTexture.Width, NativeTexture.Height);
			CompareActualSizeMetadataSize(textureSize);
		}

		public Texture2D NativeTexture { get; protected set; }

		protected override void DisposeData()
		{
			if (NativeTexture != null)
				NativeTexture.Dispose();
		}

		public override void Fill(Color[] colors)
		{
			if (PixelSize.Width * PixelSize.Height != colors.Length)
				throw new InvalidNumberOfColors(PixelSize);
			if (NativeTexture == null)
				NativeTexture = new Texture2D(nativeDevice, (int)PixelSize.Width, (int)PixelSize.Height);
			NativeTexture.SetData(ConvertToXnaColors(colors));
		}

		public override void Fill(byte[] rgbaColors)
		{
			if (PixelSize.Width * PixelSize.Height * Color.SizeInBytes != rgbaColors.Length)
				throw new InvalidNumberOfBytes(PixelSize);
			if (NativeTexture == null)
				NativeTexture = new Texture2D(nativeDevice, (int)PixelSize.Width, (int)PixelSize.Height);
			NativeTexture.SetData(rgbaColors);
		}

		public class NonAlphaByteDataTextureFillingIsNotSupportedInXna : Exception {}

		protected override void SetSamplerState() {}

		private static XnaColor[] ConvertToXnaColors(Color[] deltaColors)
		{
			var colors = new XnaColor[deltaColors.Length];
			for (int index = 0; index < deltaColors.Length; index++)
			{
				var color = deltaColors[index];
				colors[index] = new XnaColor(color.R, color.G, color.B, color.A);
			}
			return colors;
		}
	}
}