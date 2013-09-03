using System;
using DeltaEngine.Core;
using DeltaEngine.Extensions;
using DeltaEngine.Networking.Messages;

namespace DeltaEngine.Networking.Tcp
{
	/// <summary>
	/// The Current property will connect only once and is used by DeveloperOnlineContentLoader and
	/// NetworkLogger. The Editor will create its own connection and manages the connecting itself.
	/// </summary>
	public class OnlineServiceConnection : TcpSocket
	{
		//ncrunch: no coverage start
		internal static OnlineServiceConnection CreateForAppRunner(string apiKey, Settings settings,
			Action timeout, Action<string> errorHappened, Action ready)
		{
			var connection = CreateForEditor();
			connection.serverErrorHappened = errorHappened;
			connection.contentReady = ready;
			var projectName = AssemblyExtensions.GetEntryAssemblyForProjectName();
			connection.Connected += () => connection.Send(new LoginRequest(apiKey, projectName));
			connection.TimedOut += timeout;
			connection.Connect(settings.OnlineServiceIp, settings.OnlineServicePort);
			return connection;
		}

		internal static OnlineServiceConnection CreateForEditor()
		{
			var connection = new OnlineServiceConnection();
			connection.DataReceived += connection.OnDataReceived;
			return connection;
		}

		private OnlineServiceConnection() {}

		private Action<string> serverErrorHappened;
		public Action loadContentMetaData;
		public Action contentReady;

		private void OnDataReceived(object message)
		{
			var serverError = message as ServerError;
			var unknownMessage = message as UnknownMessage;
			var ready = message as ContentReady;
			if (serverError != null && serverErrorHappened != null)
				serverErrorHappened(serverError.Error);
			else if (unknownMessage != null && serverErrorHappened != null)
				serverErrorHappened(unknownMessage.Text);
			else if (message is LoginSuccessful)
			{
				IsLoggedIn = true;
				if (LoggedIn != null)
					LoggedIn();
			}
			else if (ready != null)
			{
				loadContentMetaData();
				if (contentReady != null)
					contentReady();
			}
		}

		public bool IsLoggedIn { get; private set; }

		public Action LoggedIn;
	}
}