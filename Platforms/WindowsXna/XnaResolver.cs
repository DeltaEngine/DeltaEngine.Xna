using DeltaEngine.Content.Json;
using DeltaEngine.Content.Xml;
using DeltaEngine.Graphics;
using DeltaEngine.Graphics.Xna;
using DeltaEngine.Input.Xna;
using DeltaEngine.Multimedia.Xna;
using DeltaEngine.Physics2D;
using DeltaEngine.Physics2D.Farseer;
using DeltaEngine.Physics3D;
using DeltaEngine.Physics3D.Jitter;
using DeltaEngine.Platforms.Windows;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering3D;
using Microsoft.Xna.Framework.Media;
#if !DEBUG 
using System;
using DeltaEngine.Core;
using DeltaEngine.Extensions;
using System.Windows.Forms;
#endif

namespace DeltaEngine.Platforms
{
	public class XnaResolver : AppRunner
	{
		public XnaResolver()
		{
#if DEBUG
			TryInitializeXna();
#else
			// Some machines with missing frameworks initialization will crash, we need useful errors
			try
			{
				TryInitializeXna();
			}
			catch (Exception exception)
			{
				Logger.Error(exception);
				if (StackTraceExtensions.StartedFromNCrunchOrNunitConsole)
					throw;
				MessageBox.Show(GetHintTextForKnownIssues(exception), "Fatal XNA Initialization Error",
					MessageBoxButtons.OK);
				Application.Exit();
			}
#endif
		}
#if !DEBUG
		private static string GetHintTextForKnownIssues(Exception ex)
		{
			if (ex.ToString().Contains("NoSuitableGraphicsDeviceException"))
			{
				string hintText = "Please verify that your video card supports DirectX 9.0c or higher," +
					" your driver is up to date and you have installed the latest DirectX runtime.\n\n";
				hintText += "Exception details:\n" + ex.Message;
				return hintText;
			}
			if (ex.ToString().Contains("Could not load file or assembly 'Microsoft.Xna.Framework.Game"))
			{
				string hintText = "Please install the Microsoft XNA Framework Redistributable 4.0.\n\n";
				hintText += "Exception details:\n" + ex.Message;
				return hintText;
			}
			if (ex.ToString().Contains("Could not load file or assembly 'MonoGame.Framework"))
			{
				string hintText = "Please install MonoGame 3.0.\n\n";
				hintText += "Exception details:\n" + ex.Message;
				return hintText;
			}
			return ex.Message;
		}
#endif

		private void TryInitializeXna()
		{
			RegisterCommonEngineSingletons();
			game = new XnaGame(this);
			window = new XnaWindow(game);
			window.ViewportPixelSize = settings.Resolution;
			RegisterInstance(window);
			RegisterSingleton<WindowsSystemInformation>();
			var device = new XnaDevice(game, window, settings);
			RegisterInstance(device);
			RegisterSingleton<Drawing>();
			RegisterSingleton<BatchRenderer2D>();
			RegisterSingleton<BatchRenderer3D>();
			game.StartXnaGameToInitializeGraphics();
			RegisterInstance(game);
			RegisterInstance(game.Content);
			RegisterSingleton<XnaSoundDevice>();
			RegisterSingleton<XnaScreenshotCapturer>();
			RegisterSingleton<VideoPlayer>();
			RegisterSingleton<XnaMouse>();
			RegisterSingleton<XnaKeyboard>();
			RegisterSingleton<XnaTouch>();
			RegisterSingleton<XnaGamePad>();
			Register<InputCommands>();
			if (IsAlreadyInitialized)
				throw new UnableToRegisterMoreTypesAppAlreadyStarted();
		}

		private XnaGame game;
		private XnaWindow window;

		protected override void RegisterMediaTypes()
		{
			base.RegisterMediaTypes();
			Register<XnaImage>();
			Register<XnaShader>();
			Register<XnaGeometry>();
			Register<XnaSound>();
			Register<XnaMusic>();
			Register<XnaVideo>();
			Register<XmlContent>();
			Register<JsonContent>();
		}

		protected override void RegisterPhysics()
		{
			RegisterSingleton<FarseerPhysics>();
			RegisterSingleton<JitterPhysics>();
			Register<AffixToPhysics2D>();
			Register<AffixToPhysics3D>();
		}

		/// <summary>
		/// Instead of starting the game normally and blocking we will delay the initialization in
		/// XnaGame until the game class has been constructed and the graphics soundDevice is available.
		/// </summary>
		public override void Run()
		{
			game.RunXnaGame();
			game.Dispose();
		}
	}
}