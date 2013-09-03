using DeltaEngine.Core;
using DeltaEngine.Networking.Messages;
using DeltaEngine.Networking.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Networking.Tests
{
	public class ClientTests
	{
		[Test]
		public void ConnectToServer()
		{
			var server = new MockServer();
			using (var client = new MockClient(server))
			{
				client.Connect("localhost", 1);
				Assert.IsTrue(client.IsConnected);
			}
		}

		[Test]
		public void SendTestMessageWithoutServerShouldNotCrash()
		{
			using (var client = new MockClient(null))
			{
				Assert.IsFalse(client.IsConnected);
				client.Send(new TextMessage(""));
			}
		}

		[Test]
		public void ConvertBinaryDataToArray()
		{
			var server = new MockServer();
			Assert.IsNull(server.LastMessage);
			var client = new MockClient(server);
			client.Connect("localhost", 1);
			client.Send(new TextMessage("Hi"));
			var serverMessage = server.LastMessage as TextMessage;
			byte[] byteArray = BinaryDataExtensions.ToByteArrayWithLengthHeader(serverMessage);
			const int LenghtOfNetworkMessage = 1;
			const int LenghtOfDataVersion = 4;
			const int StringLenghtByte = 1;
			const int StringIsNullBooleanByte = 1;
			int classNameLength = "TestMessage".Length + StringLenghtByte;
			int textLength = "Hi".Length + StringLenghtByte + StringIsNullBooleanByte;
			Assert.AreEqual(
				LenghtOfNetworkMessage + LenghtOfDataVersion + classNameLength + textLength,
				byteArray.Length);
		}

		[Test]
		public void SendTestMessageToServer()
		{
			var server = new MockServer();
			var client = new MockClient(server);
			client.Connect("localhost", 1);
			client.Send(new TextMessage("Hi"));
			var serverMessage = server.LastMessage as TextMessage;
			Assert.IsNotNull(serverMessage);
			Assert.AreEqual("Hi", serverMessage.Text);
		}

		[Test]
		public void ReceiveCallback()
		{
			var server = new MockServer();
			using (var client = new MockClient(server))
			{
				client.Connect("localhost", 1);
				bool messageReceived = false;
				client.DataReceived += message => messageReceived = true;
				server.SendToAllClients(new TextMessage("Doesn't matter"));
				Assert.IsTrue(messageReceived);
			}
		}
		
		[Test]
		public void ClientIsDisconnected()
		{
			var server = new MockServer();
			var client = new MockClient(server);
			bool isConnected = false;
			client.Connected += () => isConnected = true;
			client.Disconnected += () => isConnected = false;
			client.Connect("localhost", 1);
			Assert.IsTrue(isConnected);
			client.Dispose();
			Assert.IsFalse(isConnected);
		}
	}
}