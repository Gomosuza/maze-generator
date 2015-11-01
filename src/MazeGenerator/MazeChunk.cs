using MazeGenerator.Generator;
using Microsoft.Xna.Framework;
using Engine;
using Engine.Datastructures.Quadtree;
using Engine.Input;
using Engine.Rendering;
using Engine.Rendering.Brushes;
using Engine.Rendering.Meshes;
using Plane = Engine.Rendering.Meshes.Plane;

namespace MazeGenerator
{
	/// <summary>
	/// The maze chunk represents a chunk of the maze at a specific position
	/// </summary>
	public class MazeChunk : IComponent, IQuadtreeElement
	{
		#region Fields

		private readonly Mesh _floor;
		private readonly Brush _floorBrush;
		private readonly Mesh _maze;
		private readonly Pen _pen = new Pen(Color.Black);
		private readonly Brush _wallBrush;

		#endregion

		#region Constructors

		public MazeChunk(IRenderContext renderContext, Cell[,] cells)
		{
			// we want to use different textures for walls/floor
			var wallMeshBuilder = new TexturedMeshDescriptionBuilder();
			var floorMeshBuilder = new TexturedMeshDescriptionBuilder();

			const float tileSize = 4f;
			int width = cells.GetLength(0);
			int height = cells.GetLength(1);

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					var c = cells[x, y];

					var cellBox = c.GetBoundingBox();
					if (c.Mode == CellMode.Wall)
					{
						wallMeshBuilder.AddBox(cellBox, tileSize);
					}
					else if (c.Mode == CellMode.Empty)
					{
						// optimization: instead of using one plane per cell we just create one big plane for this chunk later
						//floorMeshBuilder.AddPlane(cellBox, Plane.NegativeY, false, tileSize);
					}
				}
			}

			// generate one big floor plane for this chunk

			// get a boundingbox that contains the entire chunk by using CreateMerged on two cells that are at the opposite end of the chunk
			var topLeft = cells[0, 0];
			var bottomRight = cells[width - 1, height - 1];
			var merged = BoundingBox.CreateMerged(topLeft.GetBoundingBox(), bottomRight.GetBoundingBox());

			floorMeshBuilder.AddPlane(merged, Plane.NegativeY, false, tileSize);
			BoundingBox = merged;

			_maze = renderContext.MeshCreator.CreateMesh(wallMeshBuilder);
			_floor = renderContext.MeshCreator.CreateMesh(floorMeshBuilder);
			_wallBrush = new TexturedBrush("default");
			_floorBrush = new SolidColorBrush(Color.White);
		}

		#endregion

		#region Properties

		public int Vertices => _floor.Vertices + _maze.Vertices;

		public BoundingBox BoundingBox { get; }

		#endregion

		#region Methods

		public void Render(IRenderContext renderContext, GameTime dt)
		{
			renderContext.RenderContext3D.DrawMesh(_maze, Matrix.Identity, _wallBrush, _pen);
			renderContext.RenderContext3D.DrawMesh(_floor, Matrix.Identity, _floorBrush, _pen);
		}

		public void Update(KeyboardManager keyboard, MouseManager mouse, GameTime dt)
		{
		}

		#endregion
	}
}