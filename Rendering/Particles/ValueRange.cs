using DeltaEngine.Core;
using DeltaEngine.Extensions;

namespace DeltaEngine.Rendering.Particles
{
	public struct ValueRange
	{
		public ValueRange(float minimum, float maximum)
			: this()
		{
			Start = minimum;
			End = maximum;
		}

		public float Start { get; set; }
		public float End { get; set; }

		public float GetRandomValue()
		{
			return Start.Lerp(End, Randomizer.Current.Get());
		}
	}
}
