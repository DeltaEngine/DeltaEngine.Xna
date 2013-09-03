using System;
using System.IO;
using System.Threading;
using DeltaEngine.Mocks;
using DeltaEngine.Networking.Tcp;
using Microsoft.Win32;
using NUnit.Framework;

namespace DeltaEngine.Content.Online.Tests
{
	[Ignore]
	public class DeveloperOnlineContentLoaderTests
	{
		//ncrunch: no coverage start
		[Test]
		public void ConnectToOnlineContentServiceWithoutExistingContent()
		{
			if (Directory.Exists("Content"))
				Directory.Delete("Content", true);
			bool ready = false;
			var connection = OnlineServiceConnection.CreateForAppRunner(GetApiKeyFromRegistry(),
				new MockSettings(), () => { throw new ConnectionTimedOut(); },
				error => { throw new ServerErrorReceived(error); }, () => ready = true);
			using (var loader = new DeveloperOnlineContentLoader(connection))
			{
				loader.resolver = new ContentDataResolver();
				for (int timeoutMs = 5000; timeoutMs > 0 && !ready; timeoutMs -= 10)
					Thread.Sleep(10);
				Assert.IsTrue(Directory.Exists("Content"));
				Assert.IsTrue(ContentLoader.Exists("DeltaEngineLogo"));
			}
		}

		public class ConnectionTimedOut : Exception {}

		public class ServerErrorReceived : Exception
		{
			public ServerErrorReceived(string error)
				: base(error) {}
		}

		private static string GetApiKeyFromRegistry()
		{
			string apiKey = "";
			using (var key = Registry.CurrentUser.OpenSubKey(@"Software\DeltaEngine\Editor", false))
				if (key != null)
					apiKey = (string)key.GetValue("ApiKey");
			if (string.IsNullOrEmpty(apiKey))
				throw new ApiKeyNotSetPleaseStartEditor();
			return apiKey;
		}

		private class ApiKeyNotSetPleaseStartEditor : Exception {}

		[Test]
		public void TryUseContentBeforeReadyThrows()
		{
			var connection = OnlineServiceConnection.CreateForAppRunner(GetApiKeyFromRegistry(),
				new MockSettings(), () => { }, s => { }, () => { });
			using (var loader = new DeveloperOnlineContentLoader(connection))
			{
				loader.resolver = new ContentDataResolver();
				Assert.Throws<DeveloperOnlineContentLoader.ContentNotReady>(
					() => ContentLoader.Exists("DeltaEngineLogo"));
			}
		}
	}
}