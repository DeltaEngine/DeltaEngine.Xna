using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Rendering.Cameras
{
	/// <summary>
	/// Abstract base camera class, provides some useful constants and properties
	/// that are commonly used in most camera classes.
	/// </summary>
	public abstract class Camera
	{
		protected Camera(Device device, Window window, Vector position)
		{
			this.device = device;
			FieldOfView = DefaultFieldOfView;
			FarPlane = DefaultFarPlane;
			NearPlane = DefaultNearPlane;
			Position = position;
			Current = this;
			window.ViewportSizeChanged += size => updateProjection = true;
			device.OnSet3DMode += SetMatricesToDevice;
		}

		private readonly Device device;
		public float FieldOfView { get; set; }
		public const float DefaultFieldOfView = 90.0f;
		public float FarPlane { get; set; }
		public const float DefaultFarPlane = 100.0f;
		public float NearPlane { get; set; }
		public const float DefaultNearPlane = 1.0f;
		public Vector Position { get; set; }
		public virtual Vector Target { get; set; }
		public static readonly Vector UpVector = Vector.UnitZ;
		public static Camera Current { get; private set; }
		private bool updateProjection = true;

		private void SetMatricesToDevice()
		{
			if (updateProjection)
			{
				updateProjection = false;
				device.CameraProjectionMatrix = CreateProjectionMatrix();
			}
			device.CameraViewMatrix = Matrix.CreateLookAt(Position, Target, UpVector);
		}

		protected virtual Matrix CreateProjectionMatrix()
		{
			return Matrix.CreatePerspective(FieldOfView, ScreenSpace.Current.AspectRatio, NearPlane,
				FarPlane);
		}

		public Ray ScreenPointToRay(Point screenSpacePos)
		{
			var pixelPos = ScreenSpace.Current.ToPixelSpace(screenSpacePos);
			var viewportPixelSize = ScreenSpace.Current.ToPixelSpace(ScreenSpace.Current.Viewport);
			var normalizedPoint = new Point(2.0f * (pixelPos.X / viewportPixelSize.Width) - 1.0f,
				1.0f - 2.0f * (pixelPos.Y / viewportPixelSize.Height));
			var clipSpaceNearPoint = new Vector(normalizedPoint.X, normalizedPoint.Y, NearPlane);
			var clipSpaceFarPoint = new Vector(normalizedPoint.X, normalizedPoint.Y, FarPlane);
			var viewProj = device.CameraViewMatrix * device.CameraProjectionMatrix;
			var inverseViewProj = Matrix.Invert(viewProj);
			var worldSpaceNearPoint = Matrix.TransformHomogeneousCoordinate(clipSpaceNearPoint,
				inverseViewProj);
			var worldSpaceFarPoint = Matrix.TransformHomogeneousCoordinate(clipSpaceFarPoint,
				inverseViewProj);
			return new Ray(worldSpaceNearPoint,
				Vector.Normalize(worldSpaceNearPoint - worldSpaceFarPoint));
		}

		public Point WorldToScreenPoint(Vector point)
		{
			var viewProj = device.CameraViewMatrix * device.CameraProjectionMatrix;
			var projectedPoint = Matrix.TransformHomogeneousCoordinate(point, viewProj);
			var screenSpacePoint = new Point(0.5f * projectedPoint.X + 0.5f,
				1.0f - (0.5f * projectedPoint.Y + 0.5f));
			var viewportPixelSize = ScreenSpace.Current.ToPixelSpace(ScreenSpace.Current.Viewport);
			var pixelPos = new Point(screenSpacePoint.X * viewportPixelSize.Width,
				screenSpacePoint.Y * viewportPixelSize.Height);
			return ScreenSpace.Current.FromPixelSpace(pixelPos);
		}
	}
}