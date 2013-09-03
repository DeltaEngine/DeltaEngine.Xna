using System;

namespace DeltaEngine.Networking
{
	/// <summary>
	/// Provides the networking client functionality to send and receive any data object.
	/// </summary>
	public interface Client : IDisposable
	{
		void Connect(string targetAddress, int targetPort);
		bool IsConnected { get; }
		string TargetAddress { get; }
		void Send(object message);
		event Action<object> DataReceived;
		event Action Connected;
		event Action Disconnected;
	}
}