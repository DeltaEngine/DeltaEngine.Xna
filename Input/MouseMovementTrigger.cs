using System;
using DeltaEngine.Commands;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Tracks any mouse movement, useful to update cursor positions or check hover states.
	/// </summary>
	public class MouseMovementTrigger : DragTrigger
	{
		public MouseMovementTrigger()
		{
			Start<Mouse>();
		}

		public MouseMovementTrigger(string empty)
		{
			if (!String.IsNullOrEmpty(empty))
				throw new MouseMovementTriggerHasNoParameters();
			Start<Mouse>();
		}

		public class MouseMovementTriggerHasNoParameters : Exception {}
	}
}