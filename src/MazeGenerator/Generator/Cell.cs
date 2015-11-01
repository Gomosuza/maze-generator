using Microsoft.Xna.Framework;

namespace MazeGenerator.Generator
{
	public class Cell
	{
		#region Fields

		private const float CellSize = 4f;
		private const float MazeHeight = 4f;

		#endregion

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

		#region Methods

		public BoundingBox GetBoundingBox()
		{
			var x = X;
			var y = Y;
			var minX = x * CellSize;
			var minZ = y * CellSize;
			var maxX = (x + 1) * CellSize;
			var maxZ = (y + 1) * CellSize;
			return new BoundingBox(new Vector3(minX, 0, minZ), new Vector3(maxX, MazeHeight, maxZ));
		}

		#endregion
	}
}