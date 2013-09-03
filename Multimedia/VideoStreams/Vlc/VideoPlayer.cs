using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace DeltaEngine.Multimedia.VideoStreams.Vlc
{
	internal class VideoPlayer
	{
		public VideoPlayer(IntPtr vlcHandle)
		{
			handle = libvlc_media_player_new(vlcHandle);
			Renderer = new MemoryRenderer(handle);
		}

		private readonly IntPtr handle;
		public MemoryRenderer Renderer { get; private set; }

		~VideoPlayer()
		{
			Release();
		}

		public void Open(BasicMedia media)
		{
			libvlc_media_player_set_media(handle, media.Handle);
		}

		public void Play()
		{
			libvlc_media_player_play(handle);
		}

		public int Volume
		{
			get { return libvlc_audio_get_volume(handle); }
			set { libvlc_audio_set_volume(handle, value); }
		}

		public Size GetVideoSize()
		{
			uint width;
			uint height;
			libvlc_video_get_size(handle, 0, out width, out height);
			return new Size((int)width, (int)height);
		}

		public void Stop()
		{
			libvlc_media_player_stop(handle);
		}

		public void Release()
		{
			try
			{
				libvlc_media_player_release(handle);
			}
			catch {}
		}

		[DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
		private static extern void libvlc_media_player_stop(IntPtr handle);

		[DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr libvlc_media_player_new(IntPtr vlcHandle);

		[DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
		private static extern void libvlc_media_player_play(IntPtr handle);

		[DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
		private static extern int libvlc_audio_get_volume(IntPtr handle);

		[DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
		private static extern int libvlc_audio_set_volume(IntPtr handle, int volume);

		[DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
		private static extern int libvlc_video_get_size(IntPtr handle, uint num, out uint px,
			out uint py);

		[DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
		private static extern void libvlc_media_player_set_media(IntPtr playerHandle,
			IntPtr mediaHandle);

		[DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
		private static extern void libvlc_media_player_release(IntPtr handle);
	}
}