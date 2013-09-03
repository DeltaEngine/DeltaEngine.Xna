using Microsoft.Xna.Framework.Input;
using NativeKeyboard = Microsoft.Xna.Framework.Input.Keyboard;
using XnaKeys = Microsoft.Xna.Framework.Input.Keys;

namespace DeltaEngine.Input.Xna
{
	/// <summary>
	/// Native implementation of the Keyboard interface using Xna
	/// </summary>
	public sealed class XnaKeyboard : Keyboard
	{
		public XnaKeyboard()
		{
			IsAvailable = true;
		}

		public override void Dispose()
		{
			IsAvailable = false;
		}

		protected override void UpdateKeyStates()
		{
			var keyboardState = NativeKeyboard.GetState();
			for (int i = 0; i < (int)Key.NumberOfKeys; i++)
				UpdateKeyState(i, ref keyboardState);
		}

		private void UpdateKeyState(int key, ref KeyboardState newState)
		{
			bool isXnaKeyPressed = newState.IsKeyDown((XnaKeys)key);
			keyboardStates[key] = keyboardStates[key].UpdateOnNativePressing(isXnaKeyPressed);
			if (keyboardStates[key] == State.Pressing)
				newlyPressedKeys.Add((Key)key);
		}
	}
}