namespace MazeGenerator.Generator
{
	public class Cell
	{
		#region Constructors

		public Cell(int x, int y)
		{
			X = x;
			Y = y;
			Mode = CellMode.Undefined;
		}

		#endregion

		#region Properties

		public CellMode Mode { get; set; }

		public int X { get; set; }

		public int Y { get; set; }

		#endregion
	}
}