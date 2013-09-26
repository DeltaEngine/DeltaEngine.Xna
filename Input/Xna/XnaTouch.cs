using System;
using System.Collections.Generic;
using System.Linq;
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
			touches = new TouchCollectionUpdater();
		}

		private readonly TouchCollectionUpdater touches;

		public override Vector2D GetPosition(int touchIndex)
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
			base.Update(entities);
		}

		private void UpdateTouch()
		{
			TouchCollection newTouches = TouchPanel.GetState();
			IsAvailable = newTouches.IsConnected;
			touches.UpdateAllTouches(newTouches.ToList());
		}
	}
}