using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Tests.Datatypes
{
	public class PlaneTests
	{
		[Test]
		public void EqualityOfPlanes()
		{
			const float Distance = 4.0f;
			Assert.AreEqual(new Plane(Vector.UnitZ, Distance), new Plane(Vector.UnitZ, Distance));
			Assert.AreNotEqual(new Plane(Vector.UnitZ, Distance), new Plane(Vector.UnitZ, 1));
			Assert.AreNotEqual(new Plane(Vector.UnitZ, Distance), new Plane(Vector.UnitX, Distance));
		}

		[Test]
		public void CreatePlane()
		{
			var plane = new Plane(Vector.UnitY, 0.0f);
			Assert.AreEqual(Vector.UnitY, plane.Normal);
			Assert.AreEqual(0.0f, plane.Distance);
		}

		[Test]
		public void RayPlaneIntersect()
		{
			var ray = new Ray(Vector.UnitZ, -Vector.UnitZ);
			var plane = new Plane(Vector.UnitZ, -3.0f);
			Assert.AreEqual(Vector.UnitZ * 3.0f, plane.Intersect(ray));
		}
	}
}