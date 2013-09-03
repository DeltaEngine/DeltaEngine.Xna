using System;
using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Commands;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using Microsoft.Xna.Framework.Input.Touch;

namespace DeltaEngine.Input.Xna
{
	/// <summary>
	/// Native implementation of the Touch interface using Xna.
	/// </summary>
	public sealed class XnaTouch : Touch
	{
		public XnaTouch(Window window)
		{
			TouchPanel.WindowHandle = (IntPtr)window.Handle;
			IsAvailable = TouchPanel.GetCapabilities().IsConnected;
			TouchPanel.EnabledGestures = GestureType.DoubleTap | GestureType.Flick | GestureType.Pinch;
			touches = new TouchCollectionUpdater();
		}

		private readonly TouchCollectionUpdater touches;

		public override Point GetPosition(int touchIndex)
		{
			return touches.locations[touchIndex];
		}

		public override State GetState(int touchIndex)
		{
			return touches.states[touchIndex];
		}

		public override bool IsAvailable { get; protected set; }

		public override void Dispose()
		{
			IsAvailable = false;
		}

		public override void Update(IEnumerable<Entity> entities)
		{
			UpdateTouch();
			// ReSharper disable PossibleMultipleEnumeration
			UpdateGestures(entities);
			base.Update(entities);
			// ReSharper restore PossibleMultipleEnumeration
		}

		private void UpdateTouch()
		{
			TouchCollection newTouches = TouchPanel.GetState();
			var locations = new List<TouchLocation>();
			for (int index = 0; index < newTouches.Count; index++)
				locations.Add(newTouches[index]); //ncrunch: no coverage

			IsAvailable = newTouches.IsConnected;
			// ReSharper disable ImpureMethodCallOnReadonlyValueField
			touches.UpdateAllTouches(locations);
			// ReSharper restore ImpureMethodCallOnReadonlyValueField
		}

		private static void UpdateGestures(IEnumerable<Entity> entities)
		{
			// ReSharper disable PossibleMultipleEnumeration
			while (TouchPanel.IsGestureAvailable)
				UpdateGesture(entities, TouchPanel.ReadGesture());
			// ReSharper restore PossibleMultipleEnumeration
		}

		private static void UpdateGesture(IEnumerable<Entity> entities, GestureSample gesture)
		{
			if (gesture.GestureType == GestureType.DoubleTap)
				InvokeAllTriggersOfType<TouchDoubleTapTrigger>(entities);
			else if (gesture.GestureType == GestureType.Flick)
				InvokeAllTriggersOfType<TouchFlickTrigger>(entities);
			else if (gesture.GestureType == GestureType.Pinch)
				InvokeAllTriggersOfType<TouchPinchTrigger>(entities);
		}

		private static void InvokeAllTriggersOfType<T>(IEnumerable<Entity> entities)
			where T : Trigger
		{
			foreach (var trigger in entities.OfType<T>())
				trigger.WasInvoked = true;
		}
	}
}