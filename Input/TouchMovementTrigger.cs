using System;
using DeltaEngine.Commands;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Tracks any touch movement, useful to update cursor positions.
	/// </summary>
	public class TouchMovementTrigger : DragTrigger
	{
		public TouchMovementTrigger()
		{
			Start<Touch>();
		}

		public TouchMovementTrigger(string empty)
		{
			if (!String.IsNullOrEmpty(empty))
				throw new TouchMovementTriggerHasNoParameters();
			Start<Touch>();
		}

		public class TouchMovementTriggerHasNoParameters : Exception { }
	}
}