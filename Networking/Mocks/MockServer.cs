using System.IO;
using System.Linq;
using DeltaEngine.Core;

namespace DeltaEngine.Networking.Mocks
{
	/// <summary>
	/// Mock server used in unit testing when using a real server would be much too slow.
	/// </summary>
	public class MockServer : Server
	{
		public MockServer()
		{
			ClientDataReceived += (client, message) => LastMessage = message;
		}

		public object LastMessage { get; internal set; }

		public override void Start(int listenPort)
		{
			IsRunning = true;
			ListenPort = listenPort;
		}

		public void ClientConnectedToServer(MockClient client)
		{
			connectedClients.Add(new MockConnection(client));
		}

		public void ClientDisconnectedFromServer()
		{
			connectedClients.Clear();
		}

		public void SendToAllClients(object message)
		{
			foreach (var client in connectedClients.OfType<MockClient>())
				client.Send(message);
		}

		public void ReceiveMessage(MockClient client, byte[] bytesReceived)
		{
			var total = new MemoryStream(bytesReceived);
			var reader = new BinaryReader(total);
			var messageLength = reader.ReadNumberMostlyBelow255();
			var messageBytes = reader.ReadBytes(messageLength);
			foreach (var connection in connectedClients.OfType<MockConnection>())
				if (connection.Client == client)
					OnClientDataReceived(connection, messageBytes.ToBinaryData());
		}
	}
}