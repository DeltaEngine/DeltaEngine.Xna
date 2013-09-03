using System;
using System.Runtime.InteropServices;

namespace DeltaEngine.Multimedia.VideoStreams.Vlc
{
	internal class MediaPlayerFactory : IDisposable
	{
		public MediaPlayerFactory()
		{
			string[] args = new[]
			{
				"-I", "dummy", "--ignore-config", "--plugin-path=./plugins", "--no-plugins-cache"
			};
			try
			{
				handle = libvlc_new(args.Length, args);
			}
			catch (DllNotFoundException)
			{
				throw new LibVlcNotFoundException();
			}
			if (handle == IntPtr.Zero)
				throw new LibVlcInitException();
		}

		private readonly IntPtr handle;

		private class LibVlcNotFoundException : Exception {}

		private class LibVlcInitException : Exception {}

		public VideoPlayer CreatePlayer()
		{
			return new VideoPlayer(handle);
		}

		public BasicMedia CreateMedia(string input)
		{
			return new BasicMedia(handle, input);
		}

		public void Dispose()
		{
			try
			{
				libvlc_release(handle);
			}
			catch {}
		}

		[DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr libvlc_new(int argc,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] argv);

		[DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
		public static extern void libvlc_release(IntPtr handle);
	}
}