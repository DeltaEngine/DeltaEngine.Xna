using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Tests.Datatypes
{
	public class RangeTests
	{
		[Test]
		public void CreateEmptyRange()
		{
			var range = new Range<Point>();
			Assert.AreEqual(Point.Zero, range.Start);
			Assert.AreEqual(Point.Zero, range.End);
		}

		[Test]
		public void CreateRange()
		{
			var range = new Range<Vector>(Vector.UnitX, Vector.UnitY);
			Assert.AreEqual(Vector.UnitX, range.Start);
			Assert.AreEqual(Vector.UnitY, range.End);
		}

		[Test]
		public void ChangeRange()
		{
			var range = new Range<Vector>(Vector.UnitX, 2 * Vector.UnitX);
			range.Start = Vector.UnitY;
			range.End = 2 * Vector.UnitY;
			Assert.AreEqual(Vector.UnitY, range.Start);
			Assert.AreEqual(2 * Vector.UnitY, range.End);
		}

		[Test]
		public void GetRandomValue()
		{
			var range = new Range<Vector>(Vector.UnitX, 2 * Vector.UnitX);
			var random = range.GetRandomValue();
			Assert.IsTrue(random.X >= 1.0f && random.X <= 2.0f);
		}
	}
}