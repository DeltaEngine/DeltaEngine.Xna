using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering.Cameras
{
	/// <summary>
	/// Orthogonal 3D camera.
	/// </summary>
	public class OrthoCamera : Camera
	{
		public OrthoCamera(Device device, Window window, Size size, Vector position)
			: base(device, window, position)
		{
			Size = size;
		}

		public Size Size
		{
			get { return size; }
			set
			{
				size = value;
				CreateProjectionMatrix();
			}
		}

		private Size size;

		protected override Matrix CreateProjectionMatrix()
		{
			return Matrix.CreateOrthoProjection(Size, NearPlane, FarPlane);
		}
	}
}