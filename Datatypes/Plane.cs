using System;
using System.Runtime.InteropServices;

namespace DeltaEngine.Datatypes
{
	/// <summary>
	/// Plane struct represented by a normal vector and a direction from the origin.
	/// Details can be found at: http://en.wikipedia.org/wiki/Plane_%28geometry%29
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Plane : IEquatable<Plane>
	{
		public Plane(Vector normal, float distance)
		{
			Normal = Vector.Normalize(normal);
			Distance = distance;
		}

		public Vector Normal;
		public float Distance;

		public Vector Intersect(Ray ray)
		{
			float numerator = Vector.Dot(Normal, ray.Origin) + Distance;
			float denominator = Vector.Dot(Normal, ray.Direction);
			float distance = -(numerator / denominator);
			return ray.Origin + ray.Direction * distance;
		}

		public bool Equals(Plane other)
		{
			return Normal == other.Normal && Distance == other.Distance;
		}
	}
}