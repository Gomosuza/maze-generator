namespace MazeGenerator
{
	/// <summary>
	/// Entry point.
	/// </summary>
	internal class Program
	{
		#region Methods

		private static void Main(string[] args)
		{
			var width = 100;
			var height = 100;
			using (var game = new MazeGenerator3DGame(width, height))
			{
				game.Run();
			}
		}

		#endregion
	}
}