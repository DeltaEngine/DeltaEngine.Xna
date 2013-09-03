using System;
using System.Runtime.InteropServices;
using System.Text;

namespace DeltaEngine.Multimedia.VideoStreams.Vlc
{
	internal class BasicMedia
	{
		public BasicMedia(IntPtr hMediaLib, string path)
		{
			Handle = libvlc_media_new_path(hMediaLib, Encoding.UTF8.GetBytes(path));
		}

		public IntPtr Handle { get; private set; }

		public MediaState State
		{
			get { return libvlc_media_get_state(Handle); }
		}

		public long Duration
		{
			get { return libvlc_media_get_duration(Handle); }
		}

		public void Dispose()
		{
			try
			{
				libvlc_media_release(Handle);
			}
			catch {}
		}

		[DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
		public static extern void libvlc_media_release(IntPtr handle);

		[DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
		public static extern MediaState libvlc_media_get_state(IntPtr handle);

		[DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr libvlc_media_new_path(IntPtr vlcHandle,
			[MarshalAs(UnmanagedType.LPArray)] byte[] pathBytes);

		[DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
		public static extern long libvlc_media_get_duration(IntPtr handle);
	}
}