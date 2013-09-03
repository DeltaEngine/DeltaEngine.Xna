using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.ScreenSpaces;
using Drench.Games;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace Drench.Tests.Games
{
	public class SingleHumanGameTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			Randomizer.Use(new FixedRandom(new[] { 0.1f, 0.6f, 0.7f, 0.2f }));
			InitializeMouse();
			AdvanceTimeAndUpdateEntities();
			game = new SingleHumanGame(BoardTests.Width, BoardTests.Height);
		}

		private Game game;

		private void InitializeMouse()
		{
			mouse = Resolve<Mouse>() as MockMouse;
			if (mouse != null)
				mouse.SetPosition(Point.Zero);
		}

		private MockMouse mouse;

		[Test]
		public void NewGameInstructions()
		{
			Assert.AreEqual("Try to complete the grid in the lowest number of turns!",
				game.upperText.Text);
		}

		[Test]
		public void ClickInvalidSquare()
		{
			var firstSquare = new Point(ScreenSpace.Current.Left + Game.Border + 0.01f,
				ScreenSpace.Current.Top + Game.Border + 0.01f);
			ClickMouse(firstSquare);
			Assert.AreEqual("0 turns taken - Invalid Move!", game.upperText.Text);
		}

		private void ClickMouse(Point position)
		{
			SetMouseState(State.Pressing, position);
			SetMouseState(State.Releasing, position);
		}

		private void SetMouseState(State state, Point position)
		{
			if (mouse == null)
				return; //ncrunch: no coverage
			mouse.SetPosition(position);
			mouse.SetButtonState(MouseButton.Left, state);
			AdvanceTimeAndUpdateEntities();
		}

		[Test]
		public void ClickValidSquare()
		{
			ClickMouse(Point.Half);
			Assert.AreEqual("1 turn taken", game.upperText.Text);
		}

		[Test, Category("Slow")]
		public void ClickSquaresUntilGameOver()
		{
			InitializeClicking();
			bool isFinished = false;
			game.Exited += () => isFinished = true;
			while (!isFinished)
				ClickNextSquare();
			Assert.AreEqual("Game Over! Finished in 10 turns taken", game.upperText.Text);
		}

		private void InitializeClicking()
		{
			float width = ScreenSpace.Current.Right - ScreenSpace.Current.Left - 2 * Game.Border;
			float height = ScreenSpace.Current.Bottom - ScreenSpace.Current.Top - 2 * Game.Border;
			buttonWidth = width / BoardTests.Width;
			buttonHeight = height / BoardTests.Height;
			clickX = Game.Border + buttonWidth / 2;
			clickY = ScreenSpace.Current.Top + Game.Border + buttonHeight / 2;
		}

		private float buttonWidth;
		private float buttonHeight;
		private float clickX;
		private float clickY;

		private void ClickNextSquare()
		{
			clickX += buttonWidth;
			if (clickX >= ScreenSpace.Current.Right - Game.Border)
			{
				clickX = Game.Border + buttonWidth / 2;
				clickY += buttonHeight;
				if (clickY >= ScreenSpace.Current.Bottom - Game.Border)
					clickY = ScreenSpace.Current.Top + Game.Border + buttonHeight / 2;
			}
			ClickMouse(new Point(clickX, clickY));
		}
	}
}