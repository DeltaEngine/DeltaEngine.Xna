using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering.Sprites;

namespace $safeprojectname$
{
	public class ParallaxBackground : Entity, Updateable
	{
		public ParallaxBackground(int numberOfLayers, string[] layerImageNames, float[] scrollFactors)
		{
			layers = new BackgroundLayer[numberOfLayers];
			for (int i = 0; i < numberOfLayers; i++)
			{
				layers [i] = new BackgroundLayer(layerImageNames [i], scrollFactors [i]);
			}
			RenderLayer = 0;
		}

		public int RenderLayer
		{
			get
			{
				return layers [0].RenderLayer;
			}
			set
			{
				for (int i = 0; i < layers.Length; i++)
					layers [i].RenderLayer = value + i;
			}
		}

		private readonly BackgroundLayer[] layers;

		public float BaseSpeed
		{
			get;
			set;
		}

		public void Update()
		{
			foreach (var backgroundLayer in layers)
				backgroundLayer.ScrollByBaseSpeed(BaseSpeed);
		}
		private class BackgroundLayer : Sprite
		{
			public BackgroundLayer(string imageName, float speedFactor) : 
				base(CreateMaterial(imageName), Rectangle.One)
			{
				this.speedFactor = speedFactor;
			}

			private static Material CreateMaterial(string imageName)
			{
				return new Material(Shader.Position2DUv, imageName);
			}

			private readonly float speedFactor;

			internal void ScrollByBaseSpeed(float baseSpeed)
			{
				var uv = Get<SpriteCoordinates>().UV;
				uv.Left = uv.Left + baseSpeed * speedFactor * Time.Delta;
				Set(new SpriteCoordinates(uv));
			}
		}
	}
}