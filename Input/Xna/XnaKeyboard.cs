using System.Runtime.InteropServices;
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
			bool isXnaKeyPressed = newState.IsKeyDown(ConvertKey(key));
			keyboardStates[key] = keyboardStates[key].UpdateOnNativePressing(isXnaKeyPressed);
			if (keyboardStates[key] == State.Pressing)
				newlyPressedKeys.Add((Key)key);
		}

		private static XnaKeys ConvertKey(int key)
		{
			switch ((Key)key)
			{
				case Key.Backspace:
					return XnaKeys.Back;
				case Key.Tab:
					return XnaKeys.Tab;
				case Key.Enter:
					return XnaKeys.Enter;
				case Key.Pause:
					return XnaKeys.Pause;
				case Key.CapsLock:
					return XnaKeys.CapsLock;
				case Key.Escape:
					return XnaKeys.Escape;
				case Key.Space:
					return XnaKeys.Space;
				case Key.PageUp:
					return XnaKeys.PageUp;
				case Key.PageDown:
					return XnaKeys.PageDown;
				case Key.End:
					return XnaKeys.End;
				case Key.Home:
					return XnaKeys.Home;
				case Key.CursorLeft:
					return XnaKeys.Left;
				case Key.CursorUp:
					return XnaKeys.Up;
				case Key.CursorRight:
					return XnaKeys.Right;
				case Key.CursorDown:
					return XnaKeys.Down;
				case Key.PrintScreen:
					return XnaKeys.PrintScreen;
				case Key.Insert:
					return XnaKeys.Insert;
				case Key.Delete:
					return XnaKeys.Delete;
				case Key.D0:
					return XnaKeys.D0;
				case Key.D1:
					return XnaKeys.D1;
				case Key.D2:
					return XnaKeys.D2;
				case Key.D3:
					return XnaKeys.D3;
				case Key.D4:
					return XnaKeys.D4;
				case Key.D5:
					return XnaKeys.D5;
				case Key.D6:
					return XnaKeys.D6;
				case Key.D7:
					return XnaKeys.D7;
				case Key.D8:
					return XnaKeys.D8;
				case Key.D9:
					return XnaKeys.D9;
				case Key.A:
					return XnaKeys.A;
				case Key.B:
					return XnaKeys.B;
				case Key.C:
					return XnaKeys.C;
				case Key.D:
					return XnaKeys.D;
				case Key.E:
					return XnaKeys.E;
				case Key.F:
					return XnaKeys.F;
				case Key.G:
					return XnaKeys.G;
				case Key.H:
					return XnaKeys.H;
				case Key.I:
					return XnaKeys.I;
				case Key.J:
					return XnaKeys.J;
				case Key.K:
					return XnaKeys.K;
				case Key.L:
					return XnaKeys.L;
				case Key.M:
					return XnaKeys.M;
				case Key.N:
					return XnaKeys.N;
				case Key.O:
					return XnaKeys.O;
				case Key.P:
					return XnaKeys.P;
				case Key.Q:
					return XnaKeys.Q;
				case Key.R:
					return XnaKeys.R;
				case Key.S:
					return XnaKeys.S;
				case Key.T:
					return XnaKeys.T;
				case Key.U:
					return XnaKeys.U;
				case Key.V:
					return XnaKeys.V;
				case Key.W:
					return XnaKeys.W;
				case Key.X:
					return XnaKeys.X;
				case Key.Y:
					return XnaKeys.Y;
				case Key.Z:
					return XnaKeys.Z;
			case Key.LeftWindows:
				return XnaKeys.LeftWindows;
			case Key.RightWindows:
				return XnaKeys.RightWindows;
			case Key.WindowsKey:
				return XnaKeys.LeftWindows;
			case Key.NumPad0:
				return XnaKeys.NumPad0;
			case Key.NumPad1:
				return XnaKeys.NumPad1;
			case Key.NumPad2:
				return XnaKeys.NumPad2;
			case Key.NumPad3:
				return XnaKeys.NumPad3;
			case Key.NumPad4:
				return XnaKeys.NumPad4;
			case Key.NumPad5:
				return XnaKeys.NumPad5;
			case Key.NumPad6:
				return XnaKeys.NumPad6;
			case Key.NumPad7:
				return XnaKeys.NumPad7;
			case Key.NumPad8:
				return XnaKeys.NumPad8;
			case Key.NumPad9:
				return XnaKeys.NumPad9;
			case Key.Multiply:
				return XnaKeys.Multiply;
			case Key.Add:
				return XnaKeys.Add;
			case Key.Separator:
				return XnaKeys.Separator;
			case Key.Subtract:
				return XnaKeys.Subtract;
			case Key.Decimal:
				return XnaKeys.Decimal;
			case Key.Divide:
				return XnaKeys.Divide;
			case Key.F1:
				return XnaKeys.F1;
			case Key.F2:
				return XnaKeys.F2;
			case Key.F3:
				return XnaKeys.F3;
			case Key.F4:
				return XnaKeys.F4;
			case Key.F5:
				return XnaKeys.F5;
			case Key.F6:
				return XnaKeys.F6;
			case Key.F7:
				return XnaKeys.F7;
			case Key.F8:
				return XnaKeys.F8;
			case Key.F9:
				return XnaKeys.F9;
			case Key.F10:
				return XnaKeys.F10;
			case Key.F11:
				return XnaKeys.F11;
			case Key.F12:
				return XnaKeys.F12;
			case Key.NumLock:
				return XnaKeys.NumLock;
			case Key.Scroll:
				return XnaKeys.Scroll;
			case Key.Alt:
				return XnaKeys.LeftAlt;
			case Key.LeftShift:
				return XnaKeys.LeftShift;
			case Key.RightShift:
				return XnaKeys.RightShift;
			case Key.LeftControl:
				return XnaKeys.LeftControl;
			case Key.RightControl:
				return XnaKeys.RightControl;
			case Key.LeftAlt:
				return XnaKeys.LeftAlt;
			case Key.RightAlt:
				return XnaKeys.RightAlt;
			case Key.Semicolon:
				return XnaKeys.OemSemicolon;
			case Key.Plus:
				return XnaKeys.OemPlus;
			case Key.Comma:
				return XnaKeys.OemComma;
			case Key.Minus:
				return XnaKeys.OemMinus;
			case Key.Period:
				return XnaKeys.OemPeriod;
			case Key.Question:
				return XnaKeys.OemQuestion;
			case Key.Tilde:
				return XnaKeys.OemTilde;
			case Key.ChatPadGreen:
				return XnaKeys.ChatPadGreen;
			case Key.ChatPadOrange:
				return XnaKeys.ChatPadOrange;
			case Key.OpenBrackets:
				return XnaKeys.OemOpenBrackets;
			case Key.Pipe:
				return XnaKeys.OemPipe;
			case Key.CloseBrackets:
				return XnaKeys.OemCloseBrackets;
			case Key.Quotes:
				return XnaKeys.OemQuotes;
			case Key.Backslash:
				return XnaKeys.OemBackslash;
			}
			return XnaKeys.None;
		}

		protected override bool IsCapsLocked
		{
			get { return (((ushort)GetKeyState(0x14)) & 0xffff) != 0; }
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true,
			CallingConvention = CallingConvention.Winapi)]
		private static extern short GetKeyState(int keyCode);
	}
}