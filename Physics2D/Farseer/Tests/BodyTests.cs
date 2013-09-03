using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Physics2D.Farseer.Tests
{
	public class BodyTests : TestWithMocksOrVisually
	{
		[Test]
		public void TestBodyDefaultIsNotStatic()
		{
			var physics = new FarseerPhysics();
			var body = physics.CreateCircle(45.0f);
			Assert.IsFalse(body.IsStatic);
		}

		[Test]
		public void TestBodyDefaultSetStatic()
		{
			var physics = new FarseerPhysics();
			var body = physics.CreateRectangle(new Size(45.0f, 45.0f));
			body.IsStatic = true;
			Assert.IsTrue(body.IsStatic);
		}

		[Test]
		public void TestBodyDefaultFriction()
		{
			const float DefaultFriction = 0.2f;
			var physics = new FarseerPhysics();
			var body = physics.CreateRectangle(new Size(45.0f, 45.0f));
			Assert.AreEqual(body.Friction, DefaultFriction);
		}

		[Test]
		public void TestBodySetFriction()
		{
			var physics = new FarseerPhysics();
			var body = physics.CreateRectangle(new Size(45.0f, 45.0f));
			body.Friction = 0.5f;
			Assert.AreEqual(body.Friction, 0.5f);
		}

		[Test]
		public void TestBodyDefaultPosition()
		{
			var physics = new FarseerPhysics();
			var body = physics.CreateRectangle(new Size(45.0f, 45.0f));
			Assert.AreEqual(body.Position, Point.Zero);
		}

		[Test]
		public void TestBodySetPosition()
		{
			var setPosition = new Point(100, 100);
			var physics = new FarseerPhysics();
			var body = physics.CreateRectangle(new Size(45.0f, 45.0f));
			body.Position = setPosition;
			Assert.AreEqual(body.Position, setPosition);
		}

		[Test]
		public void TestBodyRestitution()
		{
			var physics = new FarseerPhysics();
			var body = physics.CreateRectangle(new Size(45.0f, 45.0f));
			body.Restitution = 0.5f;
			Assert.AreEqual(body.Restitution, 0.5f);
		}

		[Test]
		public void TestStaticBodyNoRotation()
		{
			var physics = new FarseerPhysics();
			var body = physics.CreateRectangle(new Size(45.0f, 45.0f));
			body.IsStatic = true;
			Assert.AreEqual(body.Rotation, 0.0f);
		}

		[Test]
		public void TestApplyLinearImpulse()
		{
			var physics = new FarseerPhysics();
			var body = physics.CreateRectangle(new Size(45.0f, 45.0f));
			Assert.IsNotNull(body);
			body.ApplyLinearImpulse(Point.Zero);
		}

		[Test]
		public void TestApplyAngularImpulse()
		{
			var physics = new FarseerPhysics();
			var body = physics.CreateRectangle(new Size(45.0f, 45.0f));
			Assert.IsNotNull(body);
			body.ApplyAngularImpulse(10.0f);
		}

		[Test]
		public void TestApplyTorque()
		{
			var physics = new FarseerPhysics();
			var body = physics.CreateRectangle(new Size(45.0f, 45.0f));
			Assert.IsNotNull(body);
			body.ApplyTorque(10.0f);
		}

		[Test]
		public void TestEmptyRectangleShapeNotAllowed()
		{
			var physics = new FarseerPhysics();
			Assert.Throws<ArgumentOutOfRangeException>(() => physics.CreateRectangle(Size.Zero));
		}
	}
}