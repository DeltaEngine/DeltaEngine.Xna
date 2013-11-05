using System;
using System.Threading;
using System.Windows.Forms;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using Microsoft.Xna.Framework;
using Color = DeltaEngine.Datatypes.Color;
using Orientation = DeltaEngine.Core.Orientation;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Window support via the build-in XNA Game.Window functionality; supports fullscreen mode.
	/// </summary>
	public class XnaWindow : Window
	{
		public XnaWindow(Game game)
		{
			this.game = game;
			Title = StackTraceExtensions.GetEntryName();
			game.Window.AllowUserResizing = true;
			game.IsMouseVisible = true;
			game.Window.ClientSizeChanged += OnViewportSizeChanged;
			game.Window.OrientationChanged +=
				(sender, args) => OnOrientationChanged(GetOrientation(game.Window.CurrentOrientation));
			game.Exiting += (sender, args) => { IsClosing = true; };
			BackgroundColor = Color.Black;
		}

		private readonly Game game;

		private void OnViewportSizeChanged(object sender, EventArgs e)
		{
			if (ViewportSizeChanged != null)
				ViewportSizeChanged(ViewportPixelSize);
		}

		public event Action<Size> ViewportSizeChanged;

		public void OnOrientationChanged(Orientation obj)
		{
			Action<Orientation> handler = OrientationChanged;
			if (handler != null)
				handler(obj);
		}

		public event Action<Orientation> OrientationChanged;

		private Orientation GetOrientation(DisplayOrientation xnaOrientation)
		{
			Orientation = xnaOrientation == DisplayOrientation.LandscapeLeft ||
				xnaOrientation == DisplayOrientation.LandscapeRight
				? Orientation.Landscape : Orientation.Portrait;
			return Orientation;
		}

		public Orientation Orientation { get; private set; }

		public string Title
		{
			get { return game.Window.Title; }
			set { game.Window.Title = value; }
		}

		public bool IsVisible
		{
			get { return game.IsActive; }
		}

		public object Handle
		{
			get { return game.Window.Handle; }
		}
		Size Window.ViewportPixelSize
		{
			get { return ViewportPixelSize; }
			set { ViewportPixelSize = value; }
		}

		public Size ViewportPixelSize
		{
			get { return new Size(game.Window.ClientBounds.Width, game.Window.ClientBounds.Height); }
			set { TotalPixelSize = value; }
		}

		public Size TotalPixelSize
		{
			get { return new Size(game.Window.ClientBounds.Width, game.Window.ClientBounds.Height); }
			set
			{
				game.Window.BeginScreenDeviceChange(false);
				game.Window.EndScreenDeviceChange(game.Window.ScreenDeviceName, (int)value.Width,
					(int)value.Height);
				OnViewportSizeChanged(game.Window, EventArgs.Empty);
			}
		}

		public Vector2D PixelPosition
		{
			get { return new Vector2D(game.Window.ClientBounds.X, game.Window.ClientBounds.Y); }
			set
			{
				Control window = Control.FromHandle((IntPtr)Handle);
				int leftBorder = game.Window.ClientBounds.X - window.Location.X;
				int topBorder = game.Window.ClientBounds.Y - window.Location.Y;
				window.Location = new System.Drawing.Point((int)value.X - leftBorder,
					(int)value.Y - topBorder);
			}
		}

		public Color BackgroundColor { get; set; }

		public void SetFullscreen(Size displaySize)
		{
			IsFullscreen = true;
			rememberedWindowedSize = new Size(game.Window.ClientBounds.Width,
				game.Window.ClientBounds.Height);
			SetResolutionAndScreenMode(displaySize);
		}

		public void SetWindowed()
		{
			IsFullscreen = false;
			SetResolutionAndScreenMode(rememberedWindowedSize);
		}

		private Size rememberedWindowedSize;

		private void SetResolutionAndScreenMode(Size displaySize)
		{
			game.Window.AllowUserResizing = IsFullscreen;
			game.Window.BeginScreenDeviceChange(IsFullscreen);
			if (FullscreenChanged != null)
				FullscreenChanged(displaySize, IsFullscreen);
			game.Window.EndScreenDeviceChange(game.Window.ScreenDeviceName);
		}

		public bool IsFullscreen { get; private set; }
		public event Action<Size, bool> FullscreenChanged;

		public bool IsClosing { get; private set; }
		public bool ShowCursor
		{
			get { return game.IsMouseVisible; }
			set { game.IsMouseVisible = value; }
		}

		public string ShowMessageBox(string caption, string message, string[] buttons)
		{
			var buttonCombination = MessageBoxButtons.OK;
			if (buttons.Contains("Cancel"))
				buttonCombination = MessageBoxButtons.OKCancel;
			if (buttons.Contains("Ignore") || buttons.Contains("Abort") || buttons.Contains("Retry"))
				buttonCombination = MessageBoxButtons.AbortRetryIgnore;
			if (buttons.Contains("Yes") || buttons.Contains("No"))
				buttonCombination = MessageBoxButtons.YesNo;
			return MessageBox.Show(message, Title + " " + caption, buttonCombination).ToString();
		}

		/// <summary>
		/// Clipboard.SetText must be executed on a STA thread, which we are not, create extra thread!
		/// </summary>
		public void CopyTextToClipboard(string text)
		{
			var staThread = new Thread(new ThreadStart(delegate
			{
				try
				{
					Clipboard.SetText(text, TextDataFormat.Text);
				}
				catch (Exception)
				{
					Logger.Warning("Failed to set clipboard text: " + text);
				}
			}));
			staThread.SetApartmentState(ApartmentState.STA);
			staThread.Start();
		}

		public void Present()
		{
			FrameworkDispatcher.Update();
		}

		public void CloseAfterFrame()
		{
			IsClosing = true;
			game.Exit();
		}

		public void Dispose()
		{
			CloseAfterFrame();
			game.Exit();
		}
	}
}