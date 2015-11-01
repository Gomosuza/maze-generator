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
			int width, height;
			if (args.Length != 2 || !int.TryParse(args[0], out width) || !int.TryParse(args[1], out height))
			{
				width = 100;
				height = 100;
			}
			using (var game = new MazeGenerator3DGame(width, height))
			{
				game.Run();
			}
		}

		#endregion
	}
}