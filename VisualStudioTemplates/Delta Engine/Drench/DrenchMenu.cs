using DeltaEngine.Datatypes;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.UserInterfaces.Controls;
using $safeprojectname$.Games;
using $safeprojectname$.Logics;

namespace $safeprojectname$
{
	public class DrenchMenu
	{
		public DrenchMenu()
		{
			scene = new Scene();
			AddOptionButtons();
			AddSliders();
		}

		private void AddOptionButtons()
		{
			scene.Add(new InteractiveButton(SinglePlayerOption, "Single Player Game") {
				Clicked = StartSinglePlayerGame
			});
			scene.Add(new InteractiveButton(HumanVsDumbAiOption, "Single Player vs Dumb AI") {
				Clicked = StartHumanVsDumbAiGame
			});
			scene.Add(new InteractiveButton(HumanVsSmartAiOption, "Single Player vs Smart AI") {
				Clicked = StartHumanVsSmartAiGame
			});
			scene.Add(new InteractiveButton(TwoHumanLocalOption, "Human vs Human (local)") {
				Clicked = StartTwoHumanLocalGame
			});
		}

		private readonly Scene scene;
		private static readonly Rectangle SinglePlayerOption = new Rectangle(0.1f, 0.25f, 0.3f, 0.1f);
		private static readonly Rectangle HumanVsDumbAiOption = new Rectangle(0.1f, 0.375f, 0.3f, 0.1f);
		private static readonly Rectangle HumanVsSmartAiOption = new Rectangle(0.1f, 0.5f, 0.3f, 0.1f);
		private static readonly Rectangle TwoHumanLocalOption = new Rectangle(0.1f, 0.625f, 0.3f, 0.1f);

		private void StartSinglePlayerGame()
		{
			scene.Hide();
			game = new SingleHumanGame(boardWidth, boardHeight);
			game.Exited += scene.Show;
		}

		private Game game;
		private int boardWidth = 10;
		private int boardHeight = 10;

		private void StartHumanVsDumbAiGame()
		{
			scene.Hide();
			game = new HumanVsAiGame(new HumanVsDumbAiLogic(boardWidth, boardHeight));
			game.Exited += scene.Show;
		}

		private void StartHumanVsSmartAiGame()
		{
			scene.Hide();
			game = new HumanVsAiGame(new HumanVsSmartAiLogic(boardWidth, boardHeight));
			game.Exited += scene.Show;
		}

		private void StartTwoHumanLocalGame()
		{
			scene.Hide();
			game = new TwoHumanLocalGame(boardWidth, boardHeight);
			game.Exited += scene.Show;
		}

		private void AddSliders()
		{
			scene.Add(new Slider(WidthSliderOption) {
				MinValue = 5,
				MaxValue = 15,
				Value = boardWidth,
				ValueChanged = WidthChanged
			});
			scene.Add(new Slider(HeightSliderOption) {
				MinValue = 5,
				MaxValue = 15,
				Value = boardHeight,
				ValueChanged = HeightChanged
			});
			UpdateBoardSizeText();
			scene.Add(boardSize);
		}

		private static readonly Rectangle WidthSliderOption = new Rectangle(0.5f, 0.3f, 0.3f, 0.05f);
		private static readonly Rectangle HeightSliderOption = new Rectangle(0.5f, 0.375f, 0.3f, 0.05f);
		private readonly FontText boardSize = new FontText(FontXml.Default, "", new Rectangle(0.5f, 
			0.425f, 0.3f, 0.1f));

		private void WidthChanged(int width)
		{
			boardWidth = width;
			UpdateBoardSizeText();
		}

		private void HeightChanged(int height)
		{
			boardHeight = height;
			UpdateBoardSizeText();
		}

		private void UpdateBoardSizeText()
		{
			boardSize.Text = "Board Size: " + boardWidth + " x " + boardHeight;
		}
	}
}