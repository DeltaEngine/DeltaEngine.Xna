namespace DeltaEngine.Rendering.Particles
{
	/// <summary>
	/// Specifies how Billboards are calculated, especially used for <see cref="ParticleEmitter"/>
	/// </summary>
	public enum BillboardMode
	{
		/// <summary>
		/// Directly uses the quad space, fastest mode, only useful for 2D.
		/// </summary>
		Standard2D,
		/// <summary>
		/// Always looks to the camera in 3D. Commonly used for 3D <see cref="Billboard"/>
		/// </summary>
		CameraFacing,
		/// <summary>
		/// For billboard in the XY Plane, Z is constant, for ground effects like shock waves and explosion.
		/// </summary>
		GroundPlane,
		/// <summary>
		/// Creates a vertical billboard but rotates around Z axis towards camera
		/// </summary>
		Vertical
	}
}
