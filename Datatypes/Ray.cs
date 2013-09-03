using System;
using System.Runtime.InteropServices;

namespace DeltaEngine.Datatypes
{
	/// <summary>
	/// Ray struct, used to fire rays into a 3D scene to find out what we can
	/// hit with that ray (for mouse picking and other simple collision stuff).
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Ray : IEquatable<Ray>
	{
		public Ray(Vector origin, Vector direction)
		{
			Origin = origin;
			Direction = direction;
		}

		public Vector Origin;
		public Vector Direction;

		public bool Equals(Ray other)
		{
			return Origin == other.Origin && Direction == other.Direction;
		}
	}
}