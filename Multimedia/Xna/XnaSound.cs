﻿using System.IO;
using DeltaEngine.Extensions;
using Microsoft.Xna.Framework.Audio;

namespace DeltaEngine.Multimedia.Xna
{
	/// <summary>
	/// Native Xna implementation for sound effect playback.
	/// </summary>
	public class XnaSound : Sound
	{
		public XnaSound(string contentName)
			: base(contentName) {}

		protected override void LoadData(Stream fileData)
		{
			if (StackTraceExtensions.StartedFromNUnitConsoleButNotFromNCrunch)
				return;
			effect = SoundEffect.FromStream(fileData);
		}

		private SoundEffect effect;

		protected override void DisposeData()
		{
			base.DisposeData();
			if (effect != null)
				effect.Dispose();
			effect = null;
		}

		public override float LengthInSeconds
		{
			get { return effect != null ? (float)effect.Duration.TotalSeconds : 0.0f; }
		}

		public override void PlayInstance(SoundInstance instanceToPlay)
		{
			var effectInstance = instanceToPlay.Handle as SoundEffectInstance;
			if (effectInstance == null)
				return;
			effectInstance.Volume = instanceToPlay.Volume.Clamp(0.0f, 1.0f);
			effectInstance.Pan = instanceToPlay.Panning.Clamp(-1.0f, +1.0f);
			effectInstance.Pitch = (instanceToPlay.Pitch - 1).Clamp(-1.0f, +1.0f);
			effectInstance.Play();
		}

		public override void StopInstance(SoundInstance instanceToStop)
		{
			var effectInstance = instanceToStop.Handle as SoundEffectInstance;
			if (effectInstance != null)
				effectInstance.Stop();
		}

		protected override void CreateChannel(SoundInstance instanceToFill)
		{
			if (effect != null)
				instanceToFill.Handle = effect.CreateInstance();
		}

		protected override void RemoveChannel(SoundInstance instanceToRemove)
		{
			var effectInstance = instanceToRemove.Handle as SoundEffectInstance;
			if (effectInstance != null)
				effectInstance.Dispose();
			instanceToRemove.Handle = null;
		}

		public override bool IsPlaying(SoundInstance instance)
		{
			var effectInstance = instance.Handle as SoundEffectInstance;
			return effectInstance != null && effectInstance.State == SoundState.Playing;
		}
	}
}