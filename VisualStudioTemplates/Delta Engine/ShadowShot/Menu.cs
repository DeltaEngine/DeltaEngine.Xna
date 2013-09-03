using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.UserInterfaces.Controls;

namespace $safeprojectname$
{
	class Menu : Scene
	{
		public Menu()
		{
			AddStartButton();
			AddQuitButton();
		}

		private void AddStartButton()
		{
			var startButton = new InteractiveButton(new Rectangle(0.3f, 0.3f, 0.4f, 0.15f), "Start Game");
			startButton.Clicked += TryInvokeGameStart;
			Add(startButton);
		}

		private void TryInvokeGameStart()
		{
			if (InitGame != null)
				InitGame();
		}

		public event Action InitGame;

		private void AddQuitButton()
		{
			var quitButton = new InteractiveButton(new Rectangle(0.3f, 0.5f, 0.4f, 0.15f), "Quit Game");
			quitButton.Clicked += TryInvokeQuit;
			Add(quitButton);
		}

		private void TryInvokeQuit()
		{
			if (QuitGame != null)
				QuitGame();
		}

		public event Action QuitGame;
	}
}