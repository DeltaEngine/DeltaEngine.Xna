using System;
using DeltaEngine.Commands;

namespace DeltaEngine.Input
{
	internal class TouchRotateTrigger : DragTrigger
	{
		public TouchRotateTrigger()
		{
			Start<Touch>();
		}

		public TouchRotateTrigger(string empty)
		{
			if (!String.IsNullOrEmpty(empty))
				throw new TouchMovementTriggerHasNoParameters();
			Start<Touch>();
		}

		public class TouchMovementTriggerHasNoParameters : Exception {}
	}
}