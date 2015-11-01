using MazeGenerator.Generator;
using Microsoft.Xna.Framework;
using Engine;
using Engine.Datastructures.Quadtree;
using Engine.Input;
using Engine.Physics.Collision;
using Engine.Rendering;
using Engine.Rendering.Brushes;
using Engine.Rendering.Meshes;
using Plane = Engine.Rendering.Meshes.Plane;

namespace MazeGenerator.Entities
{
	/// <summary>
	/// The maze chunk represents a chunk of the maze at a specific position
	/// </summary>
	public class MazeChunk : IComponent, IQuadtreeElement
	{
		#region Fields

		private const float TileSize = 4f;

		/// <summary>
		/// Number that limits width & height of a single chunk.
		/// </summary>
		public const int ChunkSize = 25;

		private static readonly Color[] _colors =
		{
			Color.White,
			Color.Red,
			Color.Blue,
			Color.Yellow,
			Color.Orange,
			Color.Green,
			Color.Pink,
			Color.Gray
		};

		private readonly Mesh _floor;
		private readonly Brush _floorBrush;
		private readonly Mesh _maze;
		private readonly Pen _pen = new Pen(Color.Black);
		private readonly Brush _wallBrush;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new maze chunk by generating the appropriate mesh for the specific part of the mesh.
		/// </summary>
		/// <param name="renderContext"></param>
		/// <param name="cells">The cells from which to generate the mesh. Note that a subset of cells is used by using the indices.</param>
		/// <param name="startXIndex">The start x index inside the cell array to use.</param>
		/// <param name="startYIndex">The start y index inside the cell array to use.</param>
		/// <param name="endX">The (excluded) end x value inside the cell array. Cells from <paramref name="startXIndex"/> to <paramref name="endX"/> - 1 are used.</param>
		/// <param name="endY">The (excluded) end y value inside the cell array. Cells from <paramref name="startYIndex"/> to <paramref name="endY"/> - 1 are used.</param>
		/// <param name="id">The id is used to give each chunk floor a different color so the user can easily see where chunks end.</param>
		/// <param name="collisionEngine">The collision engine to which to add the walls to.</param>
		public MazeChunk(IRenderContext renderContext, Cell[,] cells, int startXIndex, int startYIndex, int endX, int endY, int id, CollisionEngine collisionEngine)
		{
			var floorMeshBuilder = new TexturedMeshDescriptionBuilder();

			var wallMeshBuilder = GenerateWallMesh(cells, startXIndex, startYIndex, endX, endY, collisionEngine);

			// get a boundingbox that contains the entire chunk by using CreateMerged on two cells that are at the opposite end of the chunk
			var topLeft = cells[startXIndex, startYIndex];
			var bottomRight = cells[endX - 1, endY - 1];
			var merged = BoundingBox.CreateMerged(topLeft.GetBoundingBox(), bottomRight.GetBoundingBox());

			// generate only one big floor plane for this chunk
			floorMeshBuilder.AddPlane(merged, Plane.NegativeY, false, TileSize);
			BoundingBox = merged;

			_maze = renderContext.MeshCreator.CreateMesh(wallMeshBuilder);
			_floor = renderContext.MeshCreator.CreateMesh(floorMeshBuilder);
			_wallBrush = new SolidColorBrush(Color.DarkGray);

			_floorBrush = new SolidColorBrush(_colors[id % _colors.Length]);
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

		/// <summary>
		/// two steps: first run in x direction and mark all cells that are to be merged
		/// XXX
		/// X X
		/// X X
		/// XXX
		/// 
		/// will thus turn into
		/// 
		/// X11
		/// X X
		/// X X
		/// X11
		/// 
		/// where X is a normal cell, 1 indicating "merge in X direction"
		/// 
		/// then it runs again in Y direction and turns it into
		/// 
		/// X11
		/// X X
		/// 2 2
		/// X11
		/// 
		/// where 2 indicates "merge in Y direction"
		/// </summary>
		/// <param name="cells"></param>
		/// <param name="startXIndex"></param>
		/// <param name="startYIndex"></param>
		/// <param name="endX"></param>
		/// <param name="endY"></param>
		/// <returns></returns>
		private static int[,] MergeCells(Cell[,] cells, int startXIndex, int startYIndex, int endX, int endY)
		{
			int[,] skip = new int[endX - startXIndex, endY - startYIndex];

			for (int mergeMode = 1; mergeMode <= 2; mergeMode++)
			{
				// merge everything in x direction first, then anything left in y direction
				bool mergeX = mergeMode == 1;

				for (int y = startYIndex; y < endY; y++)
				{
					for (int x = startXIndex; x < endX; x++)
					{
						var currentCell = cells[x, y];
						if (currentCell.Mode != CellMode.Wall)
						{
							// we only merge walls for now
							continue;
						}
						// if skip value is not 0 it has already been merged before
						if (skip[x - startXIndex, y - startYIndex] != 0)
						{
							if (skip[x - startXIndex, y - startYIndex] == 1 && !mergeX)
							{
								// if the value is 1 (merged in x direction) but we are in mergeY mode, then skip cell, as the algorithm does not support this
								continue;
							}
						}

						// walk either left or down depending on our merge direction and only if we there are cells left in our area
						if (mergeX)
						{
							if (x + 1 < endX)
							{
								var nextCell = cells[x + 1, y];
								// only merge if we have a wall in the next cell
								if (nextCell != null && nextCell.Mode == CellMode.Wall)
								{
									skip[x - startXIndex + 1, y - startYIndex] = mergeMode;
								}
							}
						}
						else
						{
							if (y + 1 < endY)
							{
								var nextCell = cells[x, y + 1];
								// only merge if we have a wall in the next cell
								if (nextCell != null && nextCell.Mode == CellMode.Wall)
								{
									if (skip[x - startXIndex, y - startYIndex + 1] == 1)
									{
										// if the value is 1 (merged in x direction) but we are in mergeY mode, then skip cell, as the algorithm does not support this
										continue;
									}
									skip[x - startXIndex, y - startYIndex + 1] = mergeMode;
								}
							}
						}
					}
				}
			}
			return skip;
		}

		/// <summary>
		/// This method will generate the mesh structure inside the provided boundaries. It will already reduce the vertex count by merging walls within a corridor.
		/// Note that this is not the most efficient algorithm (in regards to optimization). It is however very fast.
		/// It runs two steps: first only merging in x direction then in y direction, this will result in a mesh like this.
		/// <example>
		/// XXX
		/// X X
		/// XXX
		/// </example>
		/// into
		/// <example>
		/// 111
		/// 2 3
		/// 444
		/// </example>
		/// Note that the left side (124) could technically also be merged if the algorithm would consider faces, however since it is cell-based it will only merge each cell once.
		/// </summary>
		/// <param name="cells"></param>
		/// <param name="startXIndex"></param>
		/// <param name="startYIndex"></param>
		/// <param name="endX"></param>
		/// <param name="endY"></param>
		/// <param name="collisionEngine"></param>
		/// <returns></returns>
		public static IMeshDescription GenerateWallMesh(Cell[,] cells, int startXIndex, int startYIndex, int endX, int endY, CollisionEngine collisionEngine)
		{
			var skip = MergeCells(cells, startXIndex, startYIndex, endX, endY);
			var wallMeshBuilder = new TexturedMeshDescriptionBuilder();

			for (int y = startYIndex; y < endY; y++)
			{
				for (int x = startXIndex; x < endX; x++)
				{
					var c = cells[x, y];

					var cellBox = c.GetBoundingBox();
					if (c.Mode == CellMode.Wall)
					{
						if (skip[x - startXIndex, y - startYIndex] == 3)
						{
							// has already been merged with previous cells, do not add again
							continue;
						}
						// look at the cells to the right if they can be merged
						if (x + 1 < endX)
						{
							// walk to the right as long as possible
							int count = 0;
							int i = 1;
							while (x + i < endX && skip[x - startXIndex + i, y - startYIndex] == 1)
							{
								count = i;
								i++;
							}
							// yes, merge
							var lastCell = cells[x + count, y].GetBoundingBox();
							cellBox = BoundingBox.CreateMerged(cellBox, lastCell);
							// flag all as already merged
							if (count > 0)
							{
								do
								{
									skip[x - startXIndex + count, y - startYIndex] = 3;
									count--;
								} while (count > 0);
								wallMeshBuilder.AddBox(cellBox, TileSize);
								collisionEngine.Add(new Wall(cellBox));
								continue;
							}
						}
						// look at the cells bellow if they can be merged
						if (y + 1 < endY)
						{
							int count = 0;
							int i = 1;
							while (y + i < endY && skip[x - startXIndex, y - startYIndex + i] == 2)
							{
								count = i;
								i++;
							}
							// yes, merge
							var lastCell = cells[x, y + count].GetBoundingBox();
							cellBox = BoundingBox.CreateMerged(cellBox, lastCell);
							// flag all as already merged
							do
							{
								skip[x - startXIndex, y - startYIndex + count] = 3;
								count--;
							} while (count > 0);
							wallMeshBuilder.AddBox(cellBox, TileSize);
							collisionEngine.Add(new Wall(cellBox));
							continue;
						}

						// no merging possible, just place default cell sized box
						wallMeshBuilder.AddBox(cellBox, TileSize);
						collisionEngine.Add(new Wall(cellBox));
					}
				}
			}

			return wallMeshBuilder;
		}

		#endregion
	}
}