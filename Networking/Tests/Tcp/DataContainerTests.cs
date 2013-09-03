using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Networking.Tcp;
using NUnit.Framework;

namespace DeltaEngine.Networking.Tests.Tcp
{
	public class DataContainerTests
	{
		[SetUp]
		public void InitializeObjectsList()
		{
			collectedDataObjects = new List<MessageData>();
		}

		[Test]
		public void CollectData()
		{
			var dataCollector = new DataCollector();
			dataCollector.ObjectFinished += container => collectedDataObjects.Add(container);
			Assert.IsEmpty(collectedDataObjects);
			var bytePackages = SplitDataStream(CreateByteList(), 7, 7, 8, 1);
			foreach (byte[] package in bytePackages)
				dataCollector.ReadBytes(package, 0, package.Length);
			Assert.AreEqual(3, collectedDataObjects.Count);
		}

		private static List<byte> CreateByteList()
		{
			var byteList = new List<byte>();
			byteList.AddRange(GetTestBytesWithLengthHeader(6));
			byteList.AddRange(GetTestBytesWithLengthHeader(4));
			byteList.AddRange(GetTestBytesWithLengthHeader(10));
			Assert.AreEqual(6+4+10+3, byteList.Count);
			return byteList;
		}

		public static byte[] GetTestBytesWithLengthHeader(int numberOfWishedTestBytes)
		{
			var data = new MemoryStream();
			var writer = new BinaryWriter(data);
			writer.WriteNumberMostlyBelow255(numberOfWishedTestBytes);
			for (int num = 0; num < numberOfWishedTestBytes; num++)
				writer.Write((byte)(100 + num));
			return data.ToArray();
		}

		private IEnumerable<byte[]> SplitDataStream(List<byte> dataStream, params int[] splits)
		{
			byte[] allByteData = dataStream.ToArray();
			var bytePackages = new List<byte[]>();
			currentIndex = 0;
			foreach (var byteCount in splits)
				ExtractBytes(byteCount, allByteData, bytePackages);
			ExtractRemainingBytes(allByteData, bytePackages);
			return bytePackages;
		}

		private int currentIndex;

		private void ExtractBytes(int byteCount, byte[] allByteData, List<byte[]> bytePackages)
		{
			var package = new byte[byteCount];
			Array.Copy(allByteData, currentIndex, package, 0, package.Length);
			bytePackages.Add(package);
			currentIndex += byteCount;
		}

		private void ExtractRemainingBytes(byte[] allByteData, List<byte[]> bytePackages)
		{
			int remainingBytes = allByteData.Length - currentIndex;
			if (remainingBytes <= 0)
				return;

			var lastPackage = new byte[remainingBytes];
			Array.Copy(allByteData, currentIndex, lastPackage, 0, lastPackage.Length);
			bytePackages.Add(lastPackage);
		}

		private List<MessageData> collectedDataObjects;

		[TearDown]
		public void DisposeObjectsList()
		{
			collectedDataObjects = null;
		}
	}
}