using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace DeltaEngine.Multimedia.VideoStreams.Vlc
{
	internal sealed class MemoryRenderer : IDisposable
	{
		internal struct PixelData : IDisposable
		{
			public unsafe byte* pPixelData;
			public int size;

			public unsafe PixelData(int size)
			{
				this.size = size;
				pPixelData = (byte*)Memory.Alloc(size);
			}

			public unsafe void Dispose()
			{
				Memory.Free(pPixelData);
			}
		}

		public unsafe MemoryRenderer(IntPtr playerHandle)
		{
			this.playerHandle = playerHandle;
			lockEventHandler = OnpLock;
			unlockEventHandler = OnpUnlock;
			displayEventHandler = OnpDisplay;
			pLockCallback = Marshal.GetFunctionPointerForDelegate(lockEventHandler);
			pUnlockCallback = Marshal.GetFunctionPointerForDelegate(unlockEventHandler);
			pDisplayCallback = Marshal.GetFunctionPointerForDelegate(displayEventHandler);
		}

		private readonly IntPtr playerHandle;
		private readonly LockEventHandler lockEventHandler;
		private readonly UnlockEventHandler unlockEventHandler;
		private readonly DisplayEventHandler displayEventHandler;
		private readonly IntPtr pLockCallback;
		private readonly IntPtr pUnlockCallback;
		private readonly IntPtr pDisplayCallback;

		public Bitmap CurrentFrame
		{
			get { return GetBitmap(); }
		}

		private unsafe void* OnpLock(void* opaque, void** plane)
		{
			*(IntPtr*)plane = (IntPtr)((PixelData*)opaque)->pPixelData;
			return null;
		}

		private unsafe void OnpUnlock(void* opaque, void* picture, void** plane) {}

		private unsafe void OnpDisplay(void* opaque, void* picture)
		{
			Memory.CopyMemory(pBuffer, ((PixelData*)opaque)->pPixelData, ((PixelData*)opaque)->size);
		}

		private unsafe void* pBuffer;

		private unsafe Bitmap GetBitmap()
		{
			return new Bitmap(format.Width, format.Height, format.Pitch, format.PixelFormat,
				new IntPtr(pBuffer));
		}

		public unsafe void SetFormat(BitmapFormat newFormat)
		{
			format = newFormat;
			format.Set(playerHandle);
			pBuffer = Memory.Alloc(format.ImageSize);
			pixelData = new PixelData(format.ImageSize);
			pixelDataPtr = GCHandle.Alloc(pixelData, GCHandleType.Pinned);
			libvlc_video_set_callbacks(playerHandle, pLockCallback, pUnlockCallback, pDisplayCallback,
				pixelDataPtr.AddrOfPinnedObject());
		}

		private BitmapFormat format;
		private PixelData pixelData;
		private GCHandle pixelDataPtr;

		public unsafe void Dispose()
		{
			libvlc_video_set_callbacks(playerHandle, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
			pixelDataPtr.Free();
			pixelData.Dispose();
			Memory.Free(pBuffer);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private unsafe delegate void* LockEventHandler(void* opaque, void** plane);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private unsafe delegate void UnlockEventHandler(void* opaque, void* picture, void** plane);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private unsafe delegate void DisplayEventHandler(void* opaque, void* picture);

		[DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
		private static extern void libvlc_video_set_callbacks(IntPtr mp, IntPtr lockPtr, IntPtr unlock,
			IntPtr display, IntPtr opaque);
	}
}