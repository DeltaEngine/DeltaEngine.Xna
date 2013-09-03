using System.Net;
using DeltaEngine.Networking.Tcp;
using NUnit.Framework;

namespace DeltaEngine.Networking.Tests.Tcp
{
	[Ignore]
	public class NetworkExtensionsTests
	{
		//ncrunch: no coverage start
		[Test]
		public void TestToEndPointWithExternalAddress()
		{
			IPEndPoint endpointFromDomain = "deltaengine.net".ToEndPoint(777);
			IPEndPoint endpointFromIp = "217.91.31.182".ToEndPoint(777);
			// Allow local IP for log server
			var validEndpoints = new[] { "217.91.31.182:777", "192.168.0.9:777" };
			Assert.Contains(endpointFromDomain.ToString(), validEndpoints);
			Assert.Contains(endpointFromIp.ToString(), validEndpoints);
		}

		[Test]
		public void TestToEndPointWithLoopbackAddress()
		{
			IPEndPoint endpointFromHostname = "localhost".ToEndPoint(777);
			IPEndPoint endpointFromIp = "127.0.0.1".ToEndPoint(777);
			const string ExpectedEndpoint = "127.0.0.1:777";
			Assert.AreEqual(ExpectedEndpoint, endpointFromHostname.ToString());
			Assert.AreEqual(ExpectedEndpoint, endpointFromIp.ToString());
		}
	}
}