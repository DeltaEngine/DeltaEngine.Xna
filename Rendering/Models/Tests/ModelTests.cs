using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Cameras;
using DeltaEngine.Rendering.Shapes3D;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Models.Tests
{
	public class ModelTests : TestWithMocksOrVisually
	{
		[Test]
		public void LoadInvalidModel()
		{
			Assert.Throws<ModelData.NoMeshesGivenNeedAtLeastOne>(
				() => new Model("InvalidModel", Vector.Zero));
		}

		[Test]
		public void RenderCubeModel()
		{
			Resolve<Window>().BackgroundColor = Color.Grey;
			new LookAtCamera(Resolve<Device>(), Resolve<Window>(), Vector.One * 2.0f, Vector.Zero);
			new Model("CubeText", Vector.Zero);
		}

		[Test]
		public void RenderCubeAndConeModel()
		{
			Resolve<Window>().BackgroundColor = Color.Grey;
			new LookAtCamera(Resolve<Device>(), Resolve<Window>(), Vector.One * 2.0f, Vector.Zero);
			new Model("CubeAndCone", Vector.Zero);
		}

		[Test]
		public void RayPick()
		{
			var camera = new LookAtCamera(Resolve<Device>(), Resolve<Window>(),
				new Vector(4.0f, 8.0f, 4.0f), Vector.Zero);
			var cube = new Model(new ModelData(new Box(Vector.One, Color.Red)), Vector.Zero);
			var floor = new Plane(Vector.UnitY, 0.0f);
			new Command(point =>
			{
				var ray = camera.ScreenPointToRay(ScreenSpace.Current.ToPixelSpace(point));
				cube.Position = floor.Intersect(ray);
			}).Add(new MouseButtonTrigger(MouseButton.Left, State.Pressed));
		}
	}
}