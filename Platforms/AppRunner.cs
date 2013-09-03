using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using DeltaEngine.Content;
using DeltaEngine.Content.Online;
using DeltaEngine.Content.Xml;
using DeltaEngine.Core;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.Graphics;
using DeltaEngine.Logging;
using DeltaEngine.Networking.Tcp;
using DeltaEngine.ScreenSpaces;
using Microsoft.Win32;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Starts an application on demand by registering, resolving and running it (via EntitiesRunner)
	/// </summary>
	public abstract class AppRunner : ApproveFirstFrameScreenshot
	{
		protected void RegisterCommonEngineSingletons()
		{
			CreateDefaultLoggers();
			CreateConsoleCommandResolver();
			LoadSettingsAndCommands();
			CreateContentLoader();
			RegisterInstance(new StopwatchTime());
			RegisterSingleton<Drawing>();
			CreateEntitySystem();
			RegisterMediaTypes();
		}

		private void CreateDefaultLoggers()
		{
			instancesToDispose.Add(new TextFileLogger());
			if (ExceptionExtensions.IsDebugMode)
				instancesToDispose.Add(new ConsoleLogger());
		}

		protected readonly List<IDisposable> instancesToDispose = new List<IDisposable>();

		private void CreateConsoleCommandResolver()
		{
			var consoleCommandManager = new ConsoleCommands(new ConsoleCommandResolver(this));
			AllRegistrationCompleted +=
				() => consoleCommandManager.RegisterCommandsFromTypes(alreadyRegisteredTypes);
			RegisterInstance(consoleCommandManager);
		}

		private void CreateContentLoader()
		{
			if (ContentLoader.current == null)
				ContentLoader.current = new DeveloperOnlineContentLoader(ConnectToOnlineService());
			ContentLoader.current.resolver = new AutofacContentDataResolver(this);
			instancesToDispose.Add(ContentLoader.current);
		}

		private OnlineServiceConnection ConnectToOnlineService()
		{
			var connection = OnlineServiceConnection.CreateForAppRunner(GetApiKey(), settings, OnTimeout,
				OnError, OnReady);
			instancesToDispose.Add(connection);
			instancesToDispose.Add(new NetworkLogger(connection));
			return connection;
		}

		private string GetApiKey()
		{
			string apiKey = "";
			using (var key = Registry.CurrentUser.OpenSubKey(@"Software\DeltaEngine\Editor", false))
				if (key != null)
					apiKey = (string)key.GetValue("ApiKey");
			if (string.IsNullOrEmpty(apiKey))
				OnConnectionError("ApiKey not set. Please login with Editor to set it up.");
			return apiKey;
		}

		internal enum ExitCode
		{
			InitializationError = -1,
			UpdateAndDrawTickFailed = -2,
			ContentMissingAndApiKeyNotSet = -3
		}

		private void OnConnectionError(string errorMessage)
		{
			Logger.Warning(errorMessage);
			connectionError = errorMessage;
		}

		private string connectionError;

		private void OnTimeout()
		{
			OnConnectionError("Content Service Connection " + settings.OnlineServiceIp + ":" +
				settings.OnlineServicePort + " timed out.");
		}

		private void OnError(string serverMessage)
		{
			OnConnectionError("Server Error: " + serverMessage);
		}

		private void OnReady()
		{
			onlineServiceReadyReceived = true;
		}

		private bool onlineServiceReadyReceived;

		private void LoadSettingsAndCommands()
		{
			instancesToDispose.Add(settings = new FileSettings());
			RegisterInstance(settings);
			ContentIsReady += () => ContentLoader.Load<InputCommands>("DefaultCommands");
		}

		protected Settings settings;
		protected internal static event Action ContentIsReady;

		private void CreateEntitySystem()
		{
			instancesToDispose.Add(
				entities = new EntitiesRunner(new AutofacHandlerResolver(this), settings));
			RegisterInstance(entities);
		}

		protected EntitiesRunner entities;

		protected virtual void RegisterMediaTypes()
		{
			Register<Material>();
			Register<ImageAnimation>();
			Register<SpriteSheetAnimation>();
		}

		internal override void MakeSureContentManagerIsReady()
		{
			if (alreadyCheckedContentManagerReady)
				return;
			alreadyCheckedContentManagerReady = true;
			if (ContentLoader.current is DeveloperOnlineContentLoader && !IsEditorContentLoader())
			{
				WaitUntilContentFromOnlineServiceIsReady();
				if (!onlineServiceReadyReceived && ContentLoader.HasValidContentForStartup())
					(ContentLoader.current as DeveloperOnlineContentLoader).OnLoadContentMetaData();
			}
			if (!ContentLoader.HasValidContentForStartup())
			{
				Window.ShowMessageBox("Unable to connect to OnlineService",
					"Unable to continue: " + connectionError, new[] { "OK" });
				Environment.Exit((int)ExitCode.ContentMissingAndApiKeyNotSet);
			}
			if (ContentIsReady != null)
				ContentIsReady();
		}

		private bool alreadyCheckedContentManagerReady;

		private static bool IsEditorContentLoader()
		{
			return ContentLoader.current.GetType().FullName == "DeltaEngine.Editor.EditorContentLoader";
		}

		private void WaitUntilContentFromOnlineServiceIsReady()
		{
			if (!ContentLoader.HasValidContentForStartup())
				Logger.Info("No content available. Waiting until OnlineService sends it to us ...");
			int timeout = ContentLoader.HasValidContentForStartup() ? 10000 : 30000;
			while (String.IsNullOrEmpty(connectionError) && !onlineServiceReadyReceived &&
				(ContentLoader.current is DeveloperOnlineContentLoader) && timeout > 0)
			{
				Thread.Sleep(10);
				timeout -= 10;
			}
		}

		public virtual void Run()
		{
			do
				RunTick();
			while (!Window.IsClosing);
			Dispose();
		}

		private Window Window
		{
			get { return cachedWindow ?? (cachedWindow = Resolve<Window>()); }
		}
		private Window cachedWindow;

		internal void RunTick()
		{
			Device.Clear();
			GlobalTime.Current.Update();
			UpdateAndDrawAllEntities();
			ExecuteTestCodeAndMakeScreenshotAfterFirstFrame();
			Device.Present();
			Window.Present();
		}

		private Device Device
		{
			get { return cachedDevice ?? (cachedDevice = Resolve<Device>()); }
		}
		private Device cachedDevice;

		/// <summary>
		/// When debugging or testing crash where the actual exception happens, not here.
		/// </summary>
		private void UpdateAndDrawAllEntities()
		{
			Drawing.NumberOfDynamicVerticesDrawnThisFrame = 0;
			Drawing.NumberOfDynamicDrawCallsThisFrame = 0;
			if (Debugger.IsAttached || StackTraceExtensions.StartedFromNCrunch)
				entities.UpdateAndDrawAllEntities(Drawing.DrawEverythingInCurrentLayer);
			else
				TryUpdateAndDrawAllEntities();
		}

		private Drawing Drawing
		{
			get { return cachedDrawing ?? (cachedDrawing = Resolve<Drawing>()); }
		}
		private Drawing cachedDrawing;

		private void TryUpdateAndDrawAllEntities()
		{
			try
			{
				entities.UpdateAndDrawAllEntities(Drawing.DrawEverythingInCurrentLayer);
			}
			catch (Exception exception)
			{
				Logger.Error(exception);
				if (exception.IsWeak())
					return;
				DisplayMessageBoxAndCloseApp(exception, "Fatal Runtime Error");
			}
		}

		private void DisplayMessageBoxAndCloseApp(Exception exception, string title)
		{
			Window.CopyTextToClipboard(exception.ToString());
			if (Window.ShowMessageBox(title, "Unable to continue: " + exception,
					new[] { "Abort", "Ignore" }) == "Ignore")
				return;
			Dispose();
			if (!StackTraceExtensions.StartedFromNCrunch)
				Environment.Exit((int)ExitCode.UpdateAndDrawTickFailed);
		}

		public override void Dispose()
		{
			base.Dispose();
			foreach (var instance in instancesToDispose)
				instance.Dispose();
			instancesToDispose.Clear();
			if (ScreenSpace.Current != null)
				ScreenSpace.Current.Dispose();
		}
	}
}