using System.Collections.Generic;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using Microsoft.Xna.Framework;
using Point = DeltaEngine.Datatypes.Point;
using XnaInput = Microsoft.Xna.Framework.Input;

namespace DeltaEngine.Input.Xna
{
	/// <summary>
	/// Native implementation of the GamePad interface using Xna and PlayerIndex.One
	/// </summary>
	public class XnaGamePad : GamePad
	{
		public XnaGamePad()
			: this(GamePadNumber.Any) {}

		public XnaGamePad(GamePadNumber number)
			: base(number)
		{
			states = new State[GamePadButton.A.GetCount()];
			for (int i = 0; i < states.Length; i++)
				states[i] = State.Released;
		}

		private readonly State[] states;

		public override bool IsAvailable { get; protected set; }

		private PlayerIndex GetPlayerIndexFromNumber()
		{
			if (Number == GamePadNumber.Any)
				return GetAnyPlayerIndex();
			if (Number == GamePadNumber.Two)
				return PlayerIndex.Two;
			if (Number == GamePadNumber.Three)
				return PlayerIndex.Three;
			return Number == GamePadNumber.Four ? PlayerIndex.Four : PlayerIndex.One;
		}

		private PlayerIndex GetAnyPlayerIndex()
		{
			for (int i = 0; i < 4; i++)
				if (XnaInput.GamePad.GetState((PlayerIndex)i).IsConnected)
					return (PlayerIndex)i;
			return PlayerIndex.One;
		}

		private void UpdateValuesFromState(XnaInput.GamePadState state)
		{
			leftThumbStick.X = state.ThumbSticks.Left.X;
			leftThumbStick.Y = state.ThumbSticks.Left.Y;
			rightThumbStick.X = state.ThumbSticks.Right.X;
			rightThumbStick.Y = state.ThumbSticks.Right.Y;
			leftTrigger = state.Triggers.Left;
			rightTrigger = state.Triggers.Right;
			UpdateAllButtons(state);
		}

		private Point leftThumbStick;
		private Point rightThumbStick;
		private float leftTrigger;
		private float rightTrigger;

		private void UpdateAllButtons(XnaInput.GamePadState state)
		{
			UpdateNormalButtons(state);
			UpdateStickAndShoulderButtons(state);
			UpdateDPadButtons(state);
		}

		private void UpdateNormalButtons(XnaInput.GamePadState state)
		{
			UpdateButton(state.Buttons.A, GamePadButton.A);
			UpdateButton(state.Buttons.B, GamePadButton.B);
			UpdateButton(state.Buttons.X, GamePadButton.X);
			UpdateButton(state.Buttons.Y, GamePadButton.Y);
			UpdateButton(state.Buttons.Back, GamePadButton.Back);
			UpdateButton(state.Buttons.Start, GamePadButton.Start);
			UpdateButton(state.Buttons.BigButton, GamePadButton.BigButton);
		}

		private void UpdateStickAndShoulderButtons(XnaInput.GamePadState state)
		{
			UpdateButton(state.Buttons.LeftShoulder, GamePadButton.LeftShoulder);
			UpdateButton(state.Buttons.LeftStick, GamePadButton.LeftStick);
			UpdateButton(state.Buttons.RightShoulder, GamePadButton.RightShoulder);
			UpdateButton(state.Buttons.RightStick, GamePadButton.RightStick);
		}

		private void UpdateDPadButtons(XnaInput.GamePadState state)
		{
			UpdateButton(state.DPad.Down, GamePadButton.Down);
			UpdateButton(state.DPad.Up, GamePadButton.Up);
			UpdateButton(state.DPad.Left, GamePadButton.Left);
			UpdateButton(state.DPad.Right, GamePadButton.Right);
		}

		private void UpdateButton(XnaInput.ButtonState newState, GamePadButton button)
		{
			var buttonIndex = (int)button;
			states[buttonIndex] =
				states[buttonIndex].UpdateOnNativePressing(newState == XnaInput.ButtonState.Pressed);
		}

		public override void Dispose()
		{
			IsAvailable = false;
		}

		public override Point GetLeftThumbStick()
		{
			return leftThumbStick;
		}

		public override Point GetRightThumbStick()
		{
			return rightThumbStick;
		}

		public override float GetLeftTrigger()
		{
			return leftTrigger;
		}

		public override float GetRightTrigger()
		{
			return rightTrigger;
		}

		public override State GetButtonState(GamePadButton button)
		{
			return states[(int)button];
		}

		public override void Vibrate(float strength)
		{
			XnaInput.GamePad.SetVibration(GetPlayerIndexFromNumber(), strength, strength);
		}

		public override void Update(IEnumerable<Entity> entities)
		{
			XnaInput.GamePadState state = XnaInput.GamePad.GetState(GetPlayerIndexFromNumber());
			IsAvailable = state.IsConnected;
			if (IsAvailable)
				UpdateValuesFromState(state);
			base.Update(entities);
		}
	}
}