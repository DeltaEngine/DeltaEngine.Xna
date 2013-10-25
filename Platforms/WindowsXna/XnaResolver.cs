using DeltaEngine.Content.Xml;
using DeltaEngine.Graphics;
using DeltaEngine.Graphics.Xna;
using DeltaEngine.Input.Xna;
using DeltaEngine.Multimedia.Xna;
using DeltaEngine.Platforms.Windows;
using Microsoft.Xna.Framework.Media;

namespace DeltaEngine.Platforms
{
	internal class XnaResolver : AppRunner
	{
		public XnaResolver()
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
		
		private readonly XnaGame game;
		private readonly XnaWindow window;

		protected override void RegisterMediaTypes()
		{
			base.RegisterMediaTypes();
			Register<XnaImage>();
			Register<XnaShader>();
			Register<XnaGeometry>();
			Register<XnaSound>();
			Register<XnaMusic>();
			Register<XmlContent>();
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