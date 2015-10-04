namespace MazeGenerator
{
	internal class Program
	{
		#region Methods

		private static void Main(string[] args)
		{
			using (var game = new MazeGenerator3DGame())
			{
				game.Run();
			}
		}

		#endregion
	}
}