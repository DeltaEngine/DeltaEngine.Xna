using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Cameras.Tests
{
	public class LookAtCameraTests : TestWithMocksOrVisually
	{
		[SetUp, CloseAfterFirstFrame]
		public void InitializeEntityRunner()
		{
			new MockEntitiesRunner(typeof(MockUpdateBehavior));
		}

		private LookAtCamera CreateLookAtCamera(Vector position, Vector target)
		{
			return new LookAtCamera(Resolve<Device>(), Resolve<Window>(), position, target);
		}

		private LookAtCamera CreateLookAtCamera(Vector position, Entity3D target)
		{
			return new LookAtCamera(Resolve<Device>(), Resolve<Window>(), position, target);
		}

		[Test]
		public void PositionToTargetDistance()
		{
			var camera = CreateLookAtCamera(Vector.UnitZ * 5.0f, Vector.Zero);
			Assert.AreEqual(5.0f, camera.Distance);
		}

		[Test]
		public void RotateCamera90DegreesYAxis()
		{
			var camera = CreateLookAtCamera(Vector.UnitZ, Vector.Zero);
			camera.Rotation = new Vector(0.0f, 90.0f, 0.0f);
			Assert.AreEqual(Vector.UnitX, camera.Position);
			Assert.AreEqual(Vector.Zero, camera.Target);
		}

		[Test]
		public void LookAtEntity3D()
		{
			var entity = new Entity3D(Vector.One * 5.0f, Quaternion.Identity);
			var camera = CreateLookAtCamera(Vector.Zero, entity);
			Assert.AreEqual(camera.Target, entity.Position);
		}

		[Test]
		public void ZoomTowardTheTarget()
		{
			var camera = CreateLookAtCamera(Vector.UnitX * 2.0f, Vector.Zero);
			camera.Zoom(1.0f);
			Assert.AreEqual(Vector.UnitX, camera.Position);
		}

		[Test]
		public void ZoomOutwardTheTarget()
		{
			var camera = CreateLookAtCamera(Vector.UnitX, Vector.Zero);
			camera.Zoom(-1.0f);
			Assert.AreEqual(Vector.UnitX * 2.0f, camera.Position);
		}

		[Test]
		public void OverZoomTowardTheTarget()
		{
			var camera = CreateLookAtCamera(Vector.UnitX * 3.0f, Vector.Zero);
			camera.Zoom(100.0f);
			Assert.AreEqual(Vector.Zero, camera.Position);
		}

		[Test]
		public void RayFromScreenCenter()
		{
			var camera = CreateLookAtCamera(Vector.UnitZ, Vector.Zero);
			var ray = camera.ScreenPointToRay(Point.Half);
			//Assert.AreEqual(-Vector.UnitZ, ray.Direction);
		}

		[Test]
		public void WorldToScreenPoint()
		{
			var camera = CreateLookAtCamera(Vector.One, Vector.Zero);
			var point = camera.WorldToScreenPoint(Vector.Zero);
			Assert.AreEqual(Point.Half, point);
		}
	}
}