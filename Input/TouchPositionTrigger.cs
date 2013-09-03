using DeltaEngine.Commands;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Tracks touch movement with a mouse button in a prescribed state.
	/// </summary>
	public class TouchPositionTrigger : PositionTrigger
	{
		public TouchPositionTrigger(State state = State.Pressing)
		{
			State = state;
			Start<Touch>();
		}
		
		public State State { get; private set; }
	}
}