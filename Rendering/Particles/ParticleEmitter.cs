using System;
using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering.Sprites;

namespace DeltaEngine.Rendering.Particles
{
	/// <summary>
	/// Holds data on how to spawn particles and currently existant ones.
	/// </summary>
	public sealed class ParticleEmitter : Entity2D, Updateable
	{
		public ParticleEmitter(ParticleEmitterData data, Point spawnPosition)
			: base(new Rectangle(spawnPosition, Size.Zero))
		{
			Add(data);
			if (data.ParticleMaterial == null)
			{
				IsActive = false;
				throw new UnableToCreateWithoutMaterial();
			}
			OnDraw<ParticleRenderer>();
			if (data.ParticleMaterial.Animation != null)
				Stop<UpdateImageAnimation>();
			if (data.ParticleMaterial.SpriteSheet != null)
				Stop<UpdateSpriteSheetAnimation>();
		}

		public class UnableToCreateWithoutMaterial : Exception {}

		public void Update()
		{
			if (IsActive)
				UpdateEmitterAndParticles(Get<ParticleEmitterData>());
		}

		private void UpdateEmitterAndParticles(ParticleEmitterData data)
		{
			UpdateAndLimitNumberOfActiveParticles(data);
			UpdateAnimation(data);
			SpawnNewParticles(data);
		}

		private void UpdateAndLimitNumberOfActiveParticles(ParticleEmitterData data)
		{
			int lastIndex = -1;
			for (int index = 0; index < NumberOfActiveParticles; index++)
				if (particles[index].UpdateIfStillActive(data))
				{
					lastIndex = index;
					UpdateParticleProperties(data, index);
				}
			NumberOfActiveParticles = lastIndex + 1;
		}

		private void UpdateParticleProperties(ParticleEmitterData data, int index)
		{
			var interpolation = particles[index].ElapsedTime / data.LifeTime;
			particles[index].Color = data.Color.Start.Lerp(data.Color.End, interpolation);
			particles[index].Size = data.Size.Start.Lerp(data.Size.End, interpolation);
			particles[index].Force = data.Force.Start.Lerp(data.Force.End, interpolation);
		}

		private void UpdateAnimation(ParticleEmitterData data)
		{
			if (data.ParticleMaterial.Animation != null)
				for (int index = 0; index < NumberOfActiveParticles; index++)
					UpdateAnimationForParticle(index, data.ParticleMaterial);
			if (data.ParticleMaterial.SpriteSheet != null)
				for (int index = 0; index < NumberOfActiveParticles; index++)
					UpdateSpriteSheetAnimationForParticle(index, data.ParticleMaterial);
		}

		private void UpdateAnimationForParticle(int index, Material material)
		{
			var animationData = material.Animation;
			particles[index].CurrentFrame =
				(int)(animationData.Frames.Length * particles[index].ElapsedTime / material.Duration) %
					animationData.Frames.Length;
			particles[index].Image = animationData.Frames[particles[index].CurrentFrame];
		}

		private void UpdateSpriteSheetAnimationForParticle(int index, Material material)
		{
			var animationData = material.SpriteSheet;
			particles[index].CurrentFrame =
				(int)(animationData.UVs.Count * particles[index].ElapsedTime / material.Duration) %
					animationData.UVs.Count;
			particles[index].CurrentUV = animationData.UVs[particles[index].CurrentFrame];
		}

		private void SpawnNewParticles(ParticleEmitterData data)
		{
			if (particles == null || particles.Length != data.MaximumNumberOfParticles)
				CreateParticlesArray(data);
			ElapsedSinceLastSpawn += Time.Delta;
			if (data.SpawnInterval < 0)
			{
				if (ElapsedSinceLastSpawn > data.LifeTime)
					IsActive = false;
				return;
			}
			while (ElapsedSinceLastSpawn >= data.SpawnInterval)
			{
				SpawnOneParticle(data);
				if (data.SpawnInterval == 0)
				{
					data.SpawnInterval = -1;
					break;
				}
			}
		}

		public Particle[] particles;
		public float ElapsedSinceLastSpawn { get; set; }

		public void CreateParticlesArray(ParticleEmitterData data)
		{
			if (data.MaximumNumberOfParticles > 512)
				throw new MaximumNumberOfParticlesExceeded(data.MaximumNumberOfParticles, 512);
			particles = new Particle[data.MaximumNumberOfParticles];
			Set(particles);
		}

		private class MaximumNumberOfParticlesExceeded : Exception
		{
			public MaximumNumberOfParticlesExceeded(int specified, int maxAllowed)
				: base("Specified=" + specified + ", Maximum allowed=" + maxAllowed) {}
		}

		private void SpawnOneParticle(ParticleEmitterData data)
		{
			ElapsedSinceLastSpawn -= data.SpawnInterval;
			int freeSpot = FindFreeSpot(data);
			if (freeSpot < 0)
				return;
			particles[freeSpot].IsActive = true;
			particles[freeSpot].ElapsedTime = 0;
			particles[freeSpot].Position = Center + data.StartPosition.GetRandomValue();
			particles[freeSpot].SetStartVelocityRandomizedFromRange(data.StartVelocity.Start, data.StartVelocity.End);
			particles[freeSpot].Force = data.Force.Start;
			particles[freeSpot].Size = data.Size.Start;
			particles[freeSpot].Color = data.Color.Start;
			particles[freeSpot].Image = data.ParticleMaterial.DiffuseMap;
			particles[freeSpot].CurrentUV = data.ParticleMaterial.SpriteSheet == null
				? Rectangle.One : data.ParticleMaterial.SpriteSheet.UVs[0];
			particles[freeSpot].Rotation = data.StartRotation.GetRandomValue();
		}

		private int FindFreeSpot(ParticleEmitterData data)
		{
			for (int index = 0; index < NumberOfActiveParticles; index++)
				if (particles[index].ElapsedTime >= data.LifeTime)
					return index;
			return NumberOfActiveParticles < data.MaximumNumberOfParticles
				? NumberOfActiveParticles++ : -1;
		}

		public int NumberOfActiveParticles { get; set; }

		public void SpawnBurst(int numberOfParticles, bool destroyAfterwards = false)
		{
			var data = Get<ParticleEmitterData>();
			if (particles == null || particles.Length != data.MaximumNumberOfParticles)
				CreateParticlesArray(data);
			for (int i = 0; i < numberOfParticles; i++)
				SpawnOneParticle(data);
			if(destroyAfterwards)
				DisposeAfterSeconds(data.LifeTime);
		}

		public void DisposeAfterSeconds(float remainingSeconds)
		{
			Add(new Duration(remainingSeconds));
			Start<SelfDestructTimer>();
		}

		private class SelfDestructTimer : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (var entity in entities)
				{
					var duration = entity.Get<Duration>();
					duration.Elapsed += Time.Delta;
					if (duration.Elapsed > duration.Value)
						entity.IsActive = false;
					entity.Set(duration);
				}
			}
		}

		internal struct Duration
		{
			public Duration(float duration) :this()
			{
				Value = duration;
			}

			public float Value { get; private set; }
			public float Elapsed { get; internal set; }
		}
	}
}