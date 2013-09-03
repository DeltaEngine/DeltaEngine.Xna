using DeltaEngine.Networking.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Networking.Tests
{
	public class ServerTests
	{
		[Test]
		public void ListenForClients()
		{
			server.Start(800);
			Assert.IsTrue(server.IsRunning);
		}

		private readonly Server server = new MockServer();

		[Test]
		public void AcceptClients()
		{
			Assert.AreEqual(0, server.ListenPort);
			Assert.AreEqual(0, server.NumberOfConnectedClients);
			var client = new MockClient(server as MockServer);
			Assert.IsNotNull(client);
			client.Connect("Target", 0);
			Assert.AreEqual(1, server.NumberOfConnectedClients);
		}
	}
}