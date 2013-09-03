using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering.Particles
{
	/// <summary>
	/// Data for ParticleEmitter, usually created and saved in the Editor.
	/// </summary>
	public class ParticleEmitterData : ContentData
	{
		protected ParticleEmitterData(string contentName)
			: base(contentName) { }

		public ParticleEmitterData()
			: base("<GeneratedParticleEmitterData>")
		{
			StartVelocity = new RangeGraph<Point>(Point.Zero, Point.Zero);
			StartPosition = new RangeGraph<Point>(Point.Zero, Point.Zero);
			Force = new RangeGraph<Point>(Point.Zero, Point.Zero);
			Size = new RangeGraph<Size>(new Size(0.1f), new Size(0.1f));
			Color = new RangeGraph<Color>(Datatypes.Color.White, Datatypes.Color.White);
		}

		protected override void DisposeData(){}

		public float SpawnInterval { get; set; }
		public float LifeTime { get; set; }
		public int MaximumNumberOfParticles { get; set; }
		public RangeGraph<Point> StartPosition { get; set; }
		public RangeGraph<Point> StartVelocity { get; set; }
		public RangeGraph<Point> Force { get; set; }
		public RangeGraph<Size> Size { get; set; }
		public ValueRange StartRotation { get; set; }
		public ValueRange RotationForce { get; set; }
		public RangeGraph<Color> Color { get; set; }
		public Material ParticleMaterial { get; set; }
		public BillboardMode BillboardMode { get; set; }

		protected override void LoadData(Stream fileData)
		{
			var emitterData = (ParticleEmitterData)new BinaryReader(fileData).Create();
			SpawnInterval = emitterData.SpawnInterval;
			LifeTime = emitterData.LifeTime;
			MaximumNumberOfParticles = emitterData.MaximumNumberOfParticles;
			StartVelocity = emitterData.StartVelocity;
			Force = emitterData.Force;
			Size = emitterData.Size;
			StartRotation = emitterData.StartRotation;
			Color = emitterData.Color;
			ParticleMaterial = emitterData.ParticleMaterial;
		}
	}
}