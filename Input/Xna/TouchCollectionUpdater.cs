using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.ScreenSpaces;
using Microsoft.Xna.Framework.Input.Touch;

namespace DeltaEngine.Input.Xna
{
	/// <summary>
	/// Helper class to keep track of all touches according to their ids.
	/// </summary>
	public class TouchCollectionUpdater : TouchBase
	{
		public void UpdateAllTouches(List<TouchLocation> newTouches)
		{
			UpdatePreviouslyActiveTouches(newTouches);
			ProcessNewTouches(newTouches);
		}

		internal void UpdatePreviouslyActiveTouches(List<TouchLocation> newTouches)
		{
			for (int index = 0; index < MaxNumberOfTouches; index++)
				if (ids[index] != -1)
					UpdateTouchBy(index, newTouches);
		}

		internal void UpdateTouchBy(int index, List<TouchLocation> newTouches)
		{
			int previousNewTouchesCount = newTouches.Count;
			UpdateTouchIfPreviouslyPresent(index, newTouches);
			if (previousNewTouchesCount == newTouches.Count)
				UpdateTouchStateWithoutNewData(index);
		}

		private void UpdateTouchIfPreviouslyPresent(int index, List<TouchLocation> newTouches)
		{
			for (int newTouchIndex = 0; newTouchIndex < newTouches.Count; newTouchIndex++)
			{
				if (newTouches[newTouchIndex].Id != ids[index])
					continue;

				TouchLocation newTouch = newTouches[newTouchIndex];
				newTouches.RemoveAt(newTouchIndex);
				UpdateTouchState(index, newTouch.State);
				locations[index] = GetQuadraticPosition(newTouch);
			}
		}

		private static Vector2D GetQuadraticPosition(TouchLocation location)
		{
			return
				ScreenSpace.Current.FromPixelSpace(new Vector2D(location.Position.X, location.Position.Y));
		}

		internal void UpdateTouchStateWithoutNewData(int index)
		{
			if (states[index] == State.Releasing)
				states[index] = State.Released;
			else
				ids[index] = -1;
		}

		internal void ProcessNewTouches(List<TouchLocation> newTouches)
		{
			for (int index = 0; index < newTouches.Count; index++)
			{
				int freeIndex = FindIndexByIdOrGetFreeIndex(newTouches[index].Id);
				ids[freeIndex] = newTouches[index].Id;
				locations[freeIndex] = GetQuadraticPosition(newTouches[index]);
				states[freeIndex] = State.Pressing;
			}
		}

		internal void UpdateTouchState(int touchIndex, TouchLocationState state)
		{
			bool isTouchDown = state == TouchLocationState.Pressed || state == TouchLocationState.Moved;
			states[touchIndex] = states[touchIndex].UpdateOnNativePressing(isTouchDown);
		}
	}
}