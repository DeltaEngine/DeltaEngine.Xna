using DeltaEngine.Networking.Messages;
using DeltaEngine.Networking.Mocks;

namespace DeltaEngine.Networking.Tests.Tcp
{
	public class MockOnlineServer : MockServer
	{
		public MockOnlineServer()
		{
			ClientDataReceived += SendBackToClient;
		}

		private void SendBackToClient(Client client, object message)
		{
			if (message is LoginRequest)
				HandleLoginRequest(client, message as LoginRequest);
			else if (message is LogInfoMessage)
				HandleLogInfoMessage(client);
			else
				client.Send(new UnknownMessage(message + " is not supported"));
		}

		private void HandleLoginRequest(Client client, LoginRequest message)
		{
			if (string.IsNullOrEmpty(message.ApiKey))
				client.Send(new ServerError("Unable to login without ApiKey"));
			else
			{
				wasLoggedInSuccessfully = true;
				client.Send(new LoginSuccessful("TestUser"));
			}
		}

		private void HandleLogInfoMessage(Client client)
		{
			if (!wasLoggedInSuccessfully)
				client.Send(new ServerError("Unable to process message without login"));
		}

		private bool wasLoggedInSuccessfully;
	}
}