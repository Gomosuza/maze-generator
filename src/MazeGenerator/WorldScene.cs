using System;
using System.Collections.Generic;
using System.Linq;
using MazeGenerator.Generator;
using Microsoft.Xna.Framework;
using Engine;
using Engine.Datastructures.Quadtree;
using Engine.Diagnostics;
using Engine.Input;
using Engine.Rendering;

namespace MazeGenerator
{
	/// <summary>
	/// The world scene renders a 3D world that can be explored by the user.
	/// </summary>
	public class WorldScene : IComponent
	{
		#region Fields

		private readonly DebugMessageBuilder _messageBuilder;
		private readonly Quadtree<MazeChunk> _quadtree;
		private readonly int _totalChunks;
		private readonly int _totalVertices;
		private Cell _startCell;

		#endregion

		#region Constructors

		public WorldScene(IRenderContext renderContext, DebugMessageBuilder messageBuilder)
		{
			_messageBuilder = messageBuilder;

			var width = 50;
			var height = 50;

			var start = new Cell(0, 0).GetBoundingBox();
			var end = new Cell(width, height).GetBoundingBox();

			var fullGrid = BoundingBox.CreateMerged(start, end);
			_quadtree = new Quadtree<MazeChunk>(fullGrid);

			var cells = GenerateNewMaze(width, height);
			var chunks = GenerateChunks(renderContext, cells);

			_totalVertices = chunks.Sum(c => c.Vertices);
			_totalChunks = chunks.Count;
			foreach (var c in chunks)
			{
				_quadtree.Add(c);
			}
		}

		#endregion

		#region Methods

		public void Render(IRenderContext renderContext, GameTime dt)
		{
			var cam = renderContext.RenderContext3D.Camera;
			var frustum = new BoundingFrustum(cam.View * cam.Projection);

			var visible = _quadtree.GetIntersectingElements(frustum);
			int vertices = 0;
			int visibleChunks = 0;
			foreach (var chunk in visible)
			{
				chunk.Render(renderContext, dt);
				visibleChunks++;
				vertices += chunk.Vertices;
			}
			_messageBuilder.AppendLine($"Total Vertices: {_totalVertices}");
			_messageBuilder.AppendLine($"Visible Vertices: {vertices}");
			_messageBuilder.AppendLine($"Total chunks: {_totalChunks}");
			_messageBuilder.AppendLine($"Visible chunks: {visibleChunks}");
		}

		public void Update(KeyboardManager keyboard, MouseManager mouse, GameTime dt)
		{
		}

		private List<MazeChunk> GenerateChunks(IRenderContext renderContext, Cell[,] cells)
		{
			var chunk = new MazeChunk(renderContext, cells);
			return new List<MazeChunk>
			{
				chunk
			};
		}

		/// <summary>
		/// Generates a new grid of the given size
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		private Cell[,] GenerateNewMaze(int width, int height)
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
			_startCell = centerCell;
			return cells;
		}

		public Cell GetStartCell()
		{
			return _startCell;
		}

		#endregion
	}
}