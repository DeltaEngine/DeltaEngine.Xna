using System;
using System.Collections.Generic;

namespace DeltaEngine.Networking.Tcp
{
	internal class DataCollector
	{
		public DataCollector()
		{
			availableData = new Queue<byte>();
			currentContainerToFill = null;
		}

		//that is no good!
		private readonly Queue<byte> availableData;
		private MessageData currentContainerToFill;

		public void ReadBytes(byte[] data, int startOffset, int numberOfBytesToRead)
		{
			for (int index = startOffset; index < numberOfBytesToRead; index++)
				availableData.Enqueue(data[index]);
			ReadBytes();
		}

		public void ReadBytes()
		{
			if (currentContainerToFill == null)
			{
				int messageLength = GetMessageLength();
				if (messageLength == NotEnoughBytesForMessageLength)
					return;
				currentContainerToFill = new MessageData(messageLength);
			}
			currentContainerToFill.ReadData(availableData);
			if (currentContainerToFill.IsDataComplete)
				TriggerObjectFinishedAndResetCurrentContainer();
			if (availableData.Count > 0)
				ReadBytes();
		}

		private const int NotEnoughBytesForMessageLength = -1;

		private int GetMessageLength()
		{
			if (availableData.Count < 1)
				return NotEnoughBytesForMessageLength;
			int messageLength = availableData.Dequeue();
			if (messageLength < 255)
				return messageLength;
			if (availableData.Count < NumberOfReservedBytesForMessageLength)
				return NotEnoughBytesForMessageLength;
			return ReadLength();
		}

		private const int NumberOfReservedBytesForMessageLength = sizeof(int);

		private int ReadLength()
		{
			var lengthBuffer = new byte[NumberOfReservedBytesForMessageLength];
			for (int index = 0; index < NumberOfReservedBytesForMessageLength; index++)
				lengthBuffer[index] = availableData.Dequeue();
			int messageLength = BitConverter.ToInt32(lengthBuffer, 0);
			if (messageLength > MaxMessageLength)
				throw new MessageLengthIsTooBig(messageLength);
			return messageLength;
		}

		/// <summary>
		/// Maximum size for one packet is 128 MB! Usually something is wrong if messages get this big.
		/// </summary>
		private const int MaxMessageLength = 1024 * 1024 * 128;

		private class MessageLengthIsTooBig : Exception
		{
			public MessageLengthIsTooBig(int messageLength)
				: base(messageLength + "") {}
		}

		private void TriggerObjectFinishedAndResetCurrentContainer()
		{
			if (ObjectFinished != null)
				ObjectFinished(currentContainerToFill);
			currentContainerToFill = null;
		}

		public event Action<MessageData> ObjectFinished;
	}
}