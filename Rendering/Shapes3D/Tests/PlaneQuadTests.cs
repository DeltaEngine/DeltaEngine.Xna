using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Cameras;
using DeltaEngine.Rendering.Models;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Shapes3D.Tests
{
	internal class PlaneQuadTests : TestWithMocksOrVisually
	{
		[Test]
		public void DrawPlane()
		{
			new LookAtCamera(Resolve<Device>(), Resolve<Window>(), Vector.One, Vector.Zero);
			new Model(new ModelData(CreatePlaneQuad()), Vector.Zero);
		}

		private static PlaneQuad CreatePlaneQuad()
		{
			var material = new Material(Shader.Position3DColorUv, "DeltaEngineLogo");
			material.DefaultColor = Color.Red;
			return  new PlaneQuad(Size.Half, material);
		}
	}
}
