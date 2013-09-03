using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering.Cameras
{
	/// <summary>
	/// 3D camera that support setting of position and target.
	/// </summary>
	public class LookAtCamera : Camera
	{
		public LookAtCamera(Device device, Window window, Vector position, Vector target)
			: base(device, window, position)
		{
			// ReSharper disable DoNotCallOverridableMethodsInConstructor
			Target = target;
		}

		public LookAtCamera(Device device, Window window, Vector position, Entity3D target)
			: base(device, window, position)
		{
			entityTarget = target;
		}

		private readonly Entity3D entityTarget;

		public override Vector Target
		{
			get
			{
				if (entityTarget != null)
					return entityTarget.Position;
				return base.Target;
			}
		}

		private void UpdateInternalState()
		{
			cameraRotation = new Vector(Rotation.X, Rotation.Y.Clamp(
				MinPitchRotation, MaxPitchRotation), cameraRotation.Z);
			var rotationY = Matrix.CreateRotationY(cameraRotation.Y);
			var rotationX = Matrix.CreateRotationX(cameraRotation.X);
			var rotationMatrix = rotationX * rotationY;
			var lookVector = new Vector(0.0f, 0.0f, Distance);
			Position = rotationMatrix.TransformNormal(lookVector);
			Position = Position + Target;
		}

		public Vector Rotation
		{
			get { return cameraRotation; }
			set
			{
				cameraRotation = value;
				UpdateInternalState();
			}
		}

		private Vector cameraRotation;
		private const float MinPitchRotation = -90;
		private const float MaxPitchRotation = 90;

		public float Distance
		{
			get { return (Position - Target).Length; }
		}

		public void Zoom(float amount)
		{
			var lookDirection = Target - Position;
			var directionLength = lookDirection.Length;
			if (amount > directionLength)
				amount = directionLength - MathExtensions.Epsilon;
			lookDirection /= directionLength;
			Position = Position + lookDirection * amount;
		}
	}
}