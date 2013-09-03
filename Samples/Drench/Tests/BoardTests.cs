using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace Drench.Tests
{
	public class BoardTests
	{
		[SetUp]
		public void SetUp()
		{
			Randomizer.Use(new FixedRandom(new[] { 0.1f, 0.6f, 0.7f, 0.2f }));
			board = new Board(Width, Height);
		}

		private Board board;
		internal const int Width = 7;
		internal const int Height = 5;

		[Test]
		public void CreateBoard()
		{
			Assert.AreEqual(Width, board.Width);
			Assert.AreEqual(Height, board.Height);
		}

		[Test]
		public void BoardTopLeftColor()
		{
			Assert.AreEqual(new Color(0.5f, 1.0f, 1.0f), board.GetColor(0, 0));
		}

		[Test]
		public void Randomize()
		{
			board.Randomize();
			Assert.AreEqual(new Color(1.0f, 1.0f, 0.5f), board.GetColor(0, 0));
		}

		[Test]
		public void GetAndSetColor()
		{
			board.SetColor(0, 0, Color.Red);
			board.SetColor(Point.One, Color.Green);
			Assert.AreEqual(Color.Red, board.GetColor(0, 0));
			Assert.AreEqual(Color.Green, board.GetColor(Point.One));
		}

		[Test]
		public void CloneMatchesOriginal()
		{
			board.SetColor(0, 0, Color.Red);
			Board clone = board.Clone();
			Assert.AreEqual(Color.Red, clone.GetColor(0, 0));
		}

		[Test]
		public void ChangingOriginalAfterCloningDoesntAffectClone()
		{
			Board clone = board.Clone();
			board.SetColor(0, 0, Color.Red);
			Assert.AreNotEqual(Color.Red, clone.GetColor(0, 0));
		}
	}
}