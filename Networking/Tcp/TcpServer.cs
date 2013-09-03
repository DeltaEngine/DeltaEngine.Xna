using System;
using System.Net;

namespace DeltaEngine.Networking.Tcp
{
	/// <summary>
	/// TCP server using raw sockets via TcpServerSocket and a list of TcpSockets for the clients
	/// </summary>
	public sealed class TcpServer : Server
	{
		public override void Start(int listenPort)
		{
			socket = new TcpServerSocket(new IPEndPoint(IPAddress.Any, listenPort));
			SetUpSocketBindingAndRegisterCallback();
		}

		private TcpServerSocket socket;

		private void SetUpSocketBindingAndRegisterCallback()
		{
			socket.ClientConnected += OnClientConnected;
			socket.StartListening();
		}

		public override bool IsRunning
		{
			get { return socket.IsListening; }
		}

		public override int ListenPort
		{
			get { return socket.ListenPort; }
		}

		public override void Dispose()
		{
			socket.Dispose();
			base.Dispose();
		}
	}
}