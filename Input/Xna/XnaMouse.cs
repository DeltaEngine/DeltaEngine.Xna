using System;
using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.ScreenSpaces;
using Microsoft.Xna.Framework.Input;
using NativeMouse = Microsoft.Xna.Framework.Input.Mouse;

namespace DeltaEngine.Input.Xna
{
	/// <summary>
	/// Native implementation of the Mouse interface using Xna
	/// </summary>
	public sealed class XnaMouse : Mouse
	{
		public XnaMouse(Window window)
		{
			IsAvailable = true;
			if (window != null)
				NativeMouse.WindowHandle = (IntPtr)window.Handle;
		}

		public override bool IsAvailable { get; protected set; }

		public override void Dispose()
		{
			NativeMouse.WindowHandle = IntPtr.Zero;
		}

		public override void SetPosition(Vector2D position)
		{
			position = ScreenSpace.Current.ToPixelSpace(position);
			NativeMouse.SetPosition((int)position.X, (int)position.Y);
		}

		private void UpdateValuesFromState(ref MouseState newState)
		{
			Position = ScreenSpace.Current.FromPixelSpace(new Vector2D(newState.X, newState.Y));
			ScrollWheelValue = newState.ScrollWheelValue;
			UpdateButtonStates(ref newState);
		}

		private void UpdateButtonStates(ref MouseState newState)
		{
			LeftButton = LeftButton.UpdateOnNativePressing(newState.LeftButton == ButtonState.Pressed);
			MiddleButton =
				MiddleButton.UpdateOnNativePressing(newState.MiddleButton == ButtonState.Pressed);
			RightButton = RightButton.UpdateOnNativePressing(newState.RightButton == ButtonState.Pressed);
			X1Button = X1Button.UpdateOnNativePressing(newState.XButton1 == ButtonState.Pressed);
			X2Button = X2Button.UpdateOnNativePressing(newState.XButton2 == ButtonState.Pressed);
		}

		public override void Update(IEnumerable<Entity> entities)
		{
			MouseState newState = NativeMouse.GetState();
			UpdateValuesFromState(ref newState);
			base.Update(entities);
		}
	}
}