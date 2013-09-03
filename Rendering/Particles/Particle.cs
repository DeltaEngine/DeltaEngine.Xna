using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.Graphics.Vertices;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Rendering.Particles
{
	/// <summary>
	/// Particle data used by an Emitter to represent a single particle. Not a visual representation!
	/// </summary>
	public struct Particle : Lerp<Particle>
	{
		public Point Position { get; internal set; }
		public float Rotation { get; internal set; }
		public Point CurrentMovement { get; internal set; }
		public float ElapsedTime { get; internal set; }
		public Point Force { get; set; }
		public Size Size { get; set; }
		public Color Color { get; set; }
		public bool IsActive { get; set; }
		public Image Image { get; set; }
		public int CurrentFrame { get; set; }
		public Rectangle CurrentUV { get; set; }

		public Particle Lerp(Particle other, float interpolation)
		{
			return new Particle
			{
				Position = Position.Lerp(other.Position, interpolation),
				Rotation = Rotation.Lerp(other.Rotation, interpolation),
				CurrentMovement = CurrentMovement.Lerp(other.CurrentMovement, interpolation),
				ElapsedTime = ElapsedTime.Lerp(other.ElapsedTime, interpolation),
				Size = Size.Lerp(other.Size, interpolation),
				Color = Color.Lerp(other.Color, interpolation),
				IsActive = IsActive && other.IsActive && ElapsedTime < other.ElapsedTime,
				CurrentUV = CurrentUV.Lerp(other.CurrentUV, interpolation),
				Image = other.Image
			};
		}

		public void SetStartVelocityRandomizedFromRange(Point startVelocity,
			Point startVelocityVariance)
		{
			var velocityMin = startVelocity - startVelocityVariance;
			var velocityMax = startVelocity + startVelocityVariance;
			CurrentMovement = new Point(velocityMin.X.Lerp(velocityMax.X, Randomizer.Current.Get()),
				velocityMin.Y.Lerp(velocityMax.Y, Randomizer.Current.Get()));
		}

		internal VertexPosition2DColorUV GetTopLeftVertex()
		{
			var topLeft = new Point(Position.X - Size.Width / 2, Position.Y - Size.Height / 2);
			if (Rotation == 0)
				return new VertexPosition2DColorUV(ScreenSpace.Current.ToPixelSpace(topLeft), Color,
					CurrentUV.TopLeft);
			return
				new VertexPosition2DColorUV(
					ScreenSpace.Current.ToPixelSpace(topLeft.RotateAround(Position, Rotation)), Color,
					CurrentUV.TopLeft);
		}

		internal VertexPosition2DColorUV GetTopRightVertex()
		{
			var topRight = new Point(Position.X + Size.Width / 2, Position.Y - Size.Height / 2);
			if (Rotation == 0)
				return new VertexPosition2DColorUV(ScreenSpace.Current.ToPixelSpace(topRight), Color,
					CurrentUV.TopRight);
			return
				new VertexPosition2DColorUV(
					ScreenSpace.Current.ToPixelSpace(topRight.RotateAround(Position, Rotation)), Color,
					CurrentUV.TopRight);
		}

		internal VertexPosition2DColorUV GetBottomRightVertex()
		{
			var bottomRight = new Point(Position.X + Size.Width / 2, Position.Y + Size.Height / 2);
			if (Rotation == 0)
				return new VertexPosition2DColorUV(ScreenSpace.Current.ToPixelSpace(bottomRight), Color,
					CurrentUV.BottomRight);
			return
				new VertexPosition2DColorUV(
					ScreenSpace.Current.ToPixelSpace(bottomRight.RotateAround(Position, Rotation)), Color,
					CurrentUV.BottomRight);
		}

		internal VertexPosition2DColorUV GetBottomLeftVertex()
		{
			var bottomLeft = new Point(Position.X - Size.Width / 2, Position.Y + Size.Height / 2);
			if (Rotation == 0)
				return new VertexPosition2DColorUV(ScreenSpace.Current.ToPixelSpace(bottomLeft), Color,
					CurrentUV.BottomLeft);
			return
				new VertexPosition2DColorUV(
					ScreenSpace.Current.ToPixelSpace(bottomLeft.RotateAround(Position, Rotation)), Color,
					CurrentUV.BottomLeft);
		}

		public bool UpdateIfStillActive(ParticleEmitterData data)
		{
			ElapsedTime += Time.Delta;
			if (ElapsedTime > data.LifeTime)
				return IsActive = false;
			CurrentMovement += Force * Time.Delta;
			Position += CurrentMovement * Time.Delta;
			Rotation += data.RotationForce.GetRandomValue() * Time.Delta;
			return true;
		}
	}
}