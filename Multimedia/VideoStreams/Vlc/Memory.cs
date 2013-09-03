using System;
using System.Runtime.InteropServices;

namespace DeltaEngine.Multimedia.VideoStreams.Vlc
{
	internal class Memory
	{
		public static unsafe void* Alloc(int size)
		{
			return HeapAlloc(Ph, 8, size);
		}

		private static readonly int Ph = GetProcessHeap();

		public static unsafe void Free(void* block)
		{
			if (!HeapFree(Ph, 0, block))
				throw new InvalidOperationException();
		}

		[DllImport("kernel32")]
		private static extern int GetProcessHeap();

		[DllImport("kernel32")]
		private static extern unsafe void* HeapAlloc(int hHeap, int flags, int size);

		[DllImport("kernel32")]
		private static extern unsafe bool HeapFree(int hHeap, int flags, void* block);

		[DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = true)]
		public static extern unsafe void CopyMemory(void* dest, void* src, int size);
	}
}