using DeltaEngine.Datatypes;

namespace $safeprojectname$.Logics
{
	public class TwoHumanLocalLogic : TwoPlayerLogic
	{
		public TwoHumanLocalLogic(int width, int height) : base(width, height)
		{
		}

		public override void MakeMove(Color color)
		{
			Board.SetColor(homeSquares [ActivePlayer], color);
			ChangeActivePlayer();
			CheckForGameOver();
		}

		private void ChangeActivePlayer()
		{
			ActivePlayer = ActivePlayer == 0 ? 1 : 0;
		}

		public override void Pass()
		{
			ChangeActivePlayer();
		}
	}
}