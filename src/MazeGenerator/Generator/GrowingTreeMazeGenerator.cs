using System;
using System.Collections.Generic;
using System.Linq;

namespace MazeGenerator.Generator
{
	public class GrowingTreeMazeGenerator
	{
		#region Fields

		private static Random _random;

		#endregion

		#region Constructors

		public GrowingTreeMazeGenerator(int? seed = null)
		{
			_random = seed.HasValue ? new Random(seed.Value) : new Random();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Generates a maze.
		/// </summary>
		/// <param name="cells">The grid of cells. It must be properly initialized</param>
		/// <param name="startingCell">The cell from where to start. It must be part of <paramref name="cells"/> and must be <see cref="CellMode.Undefined"/>.</param>
		public void GenerateMaze(Cell[,] cells, Cell startingCell)
		{
			if (startingCell == null)
			{
				throw new ArgumentNullException(nameof(startingCell));
			}
			if (startingCell.Mode != CellMode.Undefined &&
			    startingCell.Mode != CellMode.Empty)
			{
				throw new ArgumentException($"{nameof(startingCell)} must be {CellMode.Undefined} or {CellMode.Empty}");
			}

			var workingList = new List<Cell>
			{
				startingCell
			};

			// begin walking from our start cell
			while (workingList.Count > 0)
			{
				// by always using recently added cell we get growing tree algorithm
				var c = workingList[workingList.Count - 1];
				// mark "working" cell as exposed, so we don't step over it multiple times (prevents loops in our maze)
				c.Mode = CellMode.Exposed;
				// get it's neighbours, but limit to neighbours which have not been walked yet
				var neighbors = GetNeighbours(cells, c).Where(n => n.Mode == CellMode.Undefined).ToList();
				if (neighbors.Count != 0)
				{
					// if we have potential neighbours, mark the current cell as empty for our maze
					c.Mode = CellMode.Empty;

					// shuffle the neighbours - this is important as we use "growing tree algorithm". if we where to not shuffle them, our maze would become really boring
					// as it would just progress into a single direction, turning only when reaching the side of the grid
					Shuffle(ref neighbors);
					neighbors.ForEach(n => n.Mode = CellMode.Exposed);
					workingList.AddRange(neighbors);
				}
				else
				{
					// without neighbours, mark the cell as a wall
					c.Mode = CellMode.Wall;
				}
				workingList.Remove(c);
			}
		}

		/// <summary>
		/// Returns the 4 neighbours (top, right, bottom, left) of any given cell.
		/// Will respect bounds and only return existing cells.
		/// </summary>
		/// <param name="cells"></param>
		/// <param name="c"></param>
		/// <returns>The list of neighours (between 2 and 4 depending on the position within the grid).</returns>
		private static List<Cell> GetNeighbours(Cell[,] cells, Cell c)
		{
			var n = new List<Cell>();
			if (c.X > 0)
			{
				n.Add(cells[c.X - 1, c.Y]);
			}
			if (c.Y > 0)
			{
				n.Add(cells[c.X, c.Y - 1]);
			}
			if (c.X < cells.GetLength(0) - 1)
			{
				n.Add(cells[c.X + 1, c.Y]);
			}
			if (c.Y < cells.GetLength(1) - 1)
			{
				n.Add(cells[c.X, c.Y + 1]);
			}
			return n;
		}

		/// <summary>
		/// Helper method to randomly shuffle a list.
		/// </summary>
		/// <param name="list"></param>
		private static void Shuffle(ref List<Cell> list)
		{
			// randomly shuffle the list
			list = list.OrderBy(c => _random.Next()).ToList();
		}

		#endregion
	}
}