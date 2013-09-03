using System.Collections.Generic;

namespace DeltaEngine.Networking.Tcp
{
	internal class MessageData
	{
		public MessageData(int dataLength)
		{
			Data = new byte[dataLength];
			readDataLength = 0;
		}

		public byte[] Data { get; private set; }
		private int readDataLength;
		
		public void ReadData(Queue<byte> availableBytes)
		{
			int allowedBytesToRead = MissingByteCount;
			if (availableBytes.Count < allowedBytesToRead)
				allowedBytesToRead = availableBytes.Count;

			for (int index = 0; index < allowedBytesToRead; index++, readDataLength++)
				Data[readDataLength] = availableBytes.Dequeue();
		}

		public int MissingByteCount
		{
			get { return Data.Length - readDataLength; }
		}

		public bool IsDataComplete
		{
			get { return readDataLength == Data.Length; }
		}
	}
}