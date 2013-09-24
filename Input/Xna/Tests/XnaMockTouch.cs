using System;
using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using Microsoft.Xna.Framework.Input.Touch;

namespace DeltaEngine.Input.Xna.Tests
{
	public class XnaMockTouch : Touch
	{
		public XnaMockTouch(Window window)
		{
			TouchPanel.WindowHandle = (IntPtr)window.Handle;
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

		public TouchCollection TouchCollection { get; set; }

		public void Run()
		{
			var locations = new List<TouchLocation>();
			for (int index = 0; index < TouchCollection.Count; index++)
				locations.Add(TouchCollection[index]);

			IsAvailable = TouchCollection.IsConnected;
			touches.UpdateAllTouches(locations);
		}

		public override bool IsAvailable
		{
			get { return TouchPanel.GetCapabilities().IsConnected; }
			protected set { }
		}

		public override void Dispose() {} //ncrunch: no coverage
		public override void Update(IEnumerable<Entity> entities) {}
	}
}