using DeltaEngine.Datatypes;

namespace Drench.Logics
{
	public abstract class TwoPlayerLogic : Logic
	{
		protected TwoPlayerLogic(int width, int height)
			: base(width, height, new[] { Point.Zero, new Point(width - 1, height - 1) }) {}
	}
}