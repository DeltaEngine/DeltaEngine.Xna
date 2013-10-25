using System.IO;
using DeltaEngine.Core;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace DeltaEngine.Graphics.Xna
{
	/// <summary>
	/// Capturing screenshots with Xna is easy in the HiDef profile, for Reach use RenderToTexture.
	/// </summary>
	public class XnaScreenshotCapturer : ScreenshotCapturer
	{
		public XnaScreenshotCapturer(Device device, Window window)
		{
			this.window = window;
			this.device = (XnaDevice)device;
		}

		private readonly XnaDevice device;
		private readonly Window window;

		public void MakeScreenshot(string fileName)
		{
			var width = (int)window.ViewportPixelSize.Width;
			var height = (int)window.ViewportPixelSize.Height;
			using (var dstTexture = new Texture2D(device.NativeDevice, width, height, false,
				device.NativeDevice.PresentationParameters.BackBufferFormat))
			{
				var pixelColors = new Color[width * height];
				device.NativeDevice.GetBackBufferData(pixelColors);
				dstTexture.SetData(pixelColors);
				using (var stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write,
					FileShare.ReadWrite))
					dstTexture.SaveAsPng(stream, width, height);
			}
		}
	}
}