using System;
using MazeGenerator.Generator;
using Microsoft.Xna.Framework;
using Engine;
using Engine.Diagnostics;
using Engine.Input;
using Engine.Rendering;
using Engine.Rendering.Brushes;
using Engine.Rendering.Meshes;
using Plane = Engine.Rendering.Meshes.Plane;

namespace MazeGenerator
{
	/// <summary>
	/// The world scene renders a 3D world that can be explored by the user.
	/// </summary>
	public class WorldScene : IComponent
	{
		#region Fields

		private const float CellSize = 4f;
		private const float MazeHeight = 4f;

		private readonly Mesh _floor;
		private readonly Brush _floorBrush;
		private readonly Mesh _maze;
		private readonly DebugMessageBuilder _messageBuilder;
		private readonly int _offsetX;
		private readonly int _offsetY;
		private readonly Brush _wallBrush;

		#endregion

		#region Constructors

		public WorldScene(IRenderContext renderContext, DebugMessageBuilder messageBuilder)
		{
			_messageBuilder = messageBuilder;
			// we want to use different textures for walls/floor
			var wallMeshBuilder = new TexturedMeshDescriptionBuilder();
			var floorMeshBuilder = new TexturedMeshDescriptionBuilder();

			Cells = GenerateNewMaze(50, 50);

			const float tileSize = 4f;
			int width = Cells.GetLength(0);
			int height = Cells.GetLength(1);

			// we want the center of the maze to be at 0,0
			_offsetX = width / 2;
			_offsetY = height / 2;
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					var c = Cells[x, y];

					var cellBox = GetBBoxForCell(c, _offsetX, _offsetY);
					if (c.Mode == CellMode.Wall)
					{
						wallMeshBuilder.AddBox(cellBox, tileSize);
					}
					else if (c.Mode == CellMode.Empty)
					{
						floorMeshBuilder.AddPlane(cellBox, Plane.NegativeY, false, tileSize);
					}
				}
			}
			_maze = renderContext.MeshCreator.CreateMesh(wallMeshBuilder);
			_floor = renderContext.MeshCreator.CreateMesh(floorMeshBuilder);
			_wallBrush = new TexturedBrush("default");
			_floorBrush = new SolidColorBrush(Color.White);
		}

		#endregion

		#region Properties

		public Cell[,] Cells { get; }

		#endregion

		#region Methods

		public void Render(IRenderContext renderContext, GameTime dt)
		{
			var p = new Pen(Color.Black);
			renderContext.RenderContext3D.DrawMesh(_maze, Matrix.Identity, _wallBrush, p);
			renderContext.RenderContext3D.DrawMesh(_floor, Matrix.Identity, _floorBrush, p);
		}

		public void Update(KeyboardManager keyboard, MouseManager mouse, GameTime dt)
		{
			var v = _maze.Vertices + _floor.Vertices;
			var p = _maze.Primitives + _floor.Primitives;
			_messageBuilder.StringBuilder.AppendLine($"Total Vertices: {v}");
			_messageBuilder.StringBuilder.AppendLine($"Total Primitives: {p}");
		}

		/// <summary>
		/// Searches the grid for an empty cell starting from the cell.
		/// Will find the closest cell to center that is empty.
		/// </summary>
		/// <returns></returns>
		public BoundingBox GetEmptyCellCloseToCenter()
		{
			int w = Cells.GetLength(0);
			int h = Cells.GetLength(1);

			var cells = Cells;

			// will spiral from center out in counter clockwise manner until first empty cell is found
			// solution from here: http://stackoverflow.com/a/31864777
			int x = 0;
			int y = 0;
			int end = Math.Max(w, h) * Math.Max(w, h);
			for (int i = 0; i < end; i++)
			{
				// Translate coordinates and mask them out.
				int xp = x + w / 2;
				int yp = y + h / 2;

				// No need to track (dx, dy) as the other examples do:
				if (Math.Abs(x) <= Math.Abs(y) && (x != y || x >= 0))
				{
					x += ((y >= 0) ? 1 : -1);
				}
				else
				{
					y += ((x >= 0) ? -1 : 1);
				}

				if (xp < 0 || xp >= w ||
				    yp < 0 || yp >= h)
				{
					continue;
				}
				Console.WriteLine($"({xp},{yp})");
				var c = cells[yp, yp];
				if (c.Mode == CellMode.Empty)
				{
					return GetBBoxForCell(c, _offsetX, _offsetY);
				}
			}
			throw new NotSupportedException("There is not a single empty cell in the grid.");
		}

		/// <summary>
		/// Generates a new grid of the given size
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		private static Cell[,] GenerateNewMaze(int width, int height)
		{
			if (width < 5 || height < 5)
			{
				throw new ArgumentException("Minimum grid size is 5x5.");
			}
			var gen = new GrowingTreeMazeGenerator();
			var cells = new Cell[width, height];
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					cells[x, y] = new Cell(x, y);
				}
			}
			var centerCell = cells[width / 2, height / 2];
			gen.GenerateMaze(cells, centerCell);
			return cells;
		}

		private static BoundingBox GetBBoxForCell(Cell cell, int offsetX, int offsetY)
		{
			var x = cell.X;
			var y = cell.Y;
			var minX = (x - offsetX) * CellSize;
			var minZ = (y - offsetY) * CellSize;
			var maxX = (x - offsetX + 1) * CellSize;
			var maxZ = (y - offsetY + 1) * CellSize;
			return new BoundingBox(new Vector3(minX, 0, minZ), new Vector3(maxX, MazeHeight, maxZ));
		}

		#endregion
	}
}