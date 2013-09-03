using DeltaEngine.Platforms;

namespace DeltaEngine.Graphics.Tests
{
	public class Program : TestWithMocksOrVisually
	{
		public static void Main()
		{
			new Program().RunTest();
		}

		private void RunTest()
		{
			InitializeResolver();
			new DrawingTests().DrawRedLine();
			//new DrawingTests().DrawVertices();
			//new ImageTests().DrawImage();
			//new MeshTests().DrawRotatingIceTower();
			RunTestAndDisposeResolverWhenDone();
		}
	}
}