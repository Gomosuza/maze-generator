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

			var width = 250;
			var height = 250;

			var start = new Cell(0, 0).GetBoundingBox();
			var end = new Cell(width - 1, height - 1).GetBoundingBox();

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

		/// <summary>
		/// Splits the maze into chunks.
		/// </summary>
		/// <param name="renderContext"></param>
		/// <param name="cells"></param>
		/// <returns></returns>
		private List<MazeChunk> GenerateChunks(IRenderContext renderContext, Cell[,] cells)
		{
			var maxSize = MazeChunk.ChunkSize;
			// instead of creating arrays per chunk we point to the source array by using indices, this is far faster and saves ram
			var w = cells.GetLength(0);
			var h = cells.GetLength(1);

			var chunks = new List<MazeChunk>();
			int yCount = (int)Math.Ceiling(h / (float)maxSize);
			int xCount = (int)Math.Ceiling(w / (float)maxSize);
			for (int y = 0; y < yCount; y++)
			{
				for (int x = 0; x < xCount; x++)
				{
					var startX = x * maxSize;
					var startY = y * maxSize;
					var endX = Math.Min((x + 1) * maxSize, w);
					var endY = Math.Min((y + 1) * maxSize, h);

					var id = y * yCount + x;
					var chunk = new MazeChunk(renderContext, cells, startX, startY, endX, endY, id);
					chunks.Add(chunk);
				}
			}
			return chunks;
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

			// TODO: seed while debugging, remove before release
			var seed = 1;
			var gen = new GrowingTreeMazeGenerator(seed);
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