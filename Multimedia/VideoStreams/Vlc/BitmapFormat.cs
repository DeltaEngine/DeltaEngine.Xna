using System;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

namespace DeltaEngine.Multimedia.VideoStreams.Vlc
{
	internal class BitmapFormat
	{
		public BitmapFormat(int width, int height, ChromaType chroma)
		{
			Width = width;
			Height = height;
			int bitsPerPixel = Init(chroma);
			chromaName = chroma.ToString();
			Pitch = Width * (bitsPerPixel / 8);
			ImageSize = Pitch * Height;
		}

		public int Width { get; private set; }
		public int Height { get; private set; }
		private readonly string chromaName;
		public int Pitch { get; private set; }
		public int ImageSize { get; private set; }
		public PixelFormat PixelFormat { get; private set; }

		private int Init(ChromaType chroma)
		{
			switch (chroma)
			{
			case ChromaType.RV15:
				PixelFormat = PixelFormat.Format16bppRgb555;
				return 16;
			case ChromaType.RV16:
				PixelFormat = PixelFormat.Format16bppRgb565;
				return 16;
			case ChromaType.RV24:
				PixelFormat = PixelFormat.Format24bppRgb;
				return 24;
			case ChromaType.RV32:
				PixelFormat = PixelFormat.Format32bppRgb;
				return 32;
			case ChromaType.RGBA:
				PixelFormat = PixelFormat.Format32bppArgb;
				return 32;
			default:
				throw new ArgumentException("Unsupported chroma type " + chroma);
			}
		}

		public void Set(IntPtr playerHandle)
		{
			libvlc_video_set_format(playerHandle, Encoding.UTF8.GetBytes(chromaName), Width, Height,
				Pitch);
		}

		[DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
		private static extern void libvlc_video_set_format(IntPtr mp,
			[MarshalAs(UnmanagedType.LPArray)] byte[] chroma, int width, int height, int pitch);
	}
}