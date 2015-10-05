using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering.Meshes
{
	/// <summary>
	/// The mesh builder allows the creation of meshes by adding various forms to itself.
	/// By passing it to a <see cref="IMeshCreator"/> a mesh is created by copying the data from its current description.
	/// Use instance methods such as <see cref="AddBox"/> to quickly build your mesh.
	/// This type will always use <see cref="PrimitiveType.TriangleList"/>.
	/// </summary>
	public sealed class MeshDescriptionBuilder : IMeshDescription
	{
		#region Fields

		private readonly List<VertexPositionTexture> _vertices;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new mesh description builder.
		/// This type will always use <see cref="PrimitiveType.TriangleList"/>.
		/// </summary>
		public MeshDescriptionBuilder()
		{
			// no need to support other types, we always use this
			// if the user wants other type he can implement it himself
			PrimitiveType = PrimitiveType.TriangleList;
			_vertices = new List<VertexPositionTexture>();
		}

		#endregion

		#region Properties

		public PrimitiveType PrimitiveType { get; }

		public int VertexCount => _vertices.Count;

		public VertexPositionTexture[] Vertices => _vertices.ToArray();

		#endregion

		#region Methods

		/// <summary>
		/// Method that allows to add a box to the mesh.
		/// As with all boxes it is outwards facing.
		/// </summary>
		/// <param name="box">The bounding box to add to the mesh.</param>
		/// <param name="tileSize">The texture scaling. The scale is applied equaly on all faces.</param>
		public void AddBox(BoundingBox box, float tileSize = 1.0f)
		{
			AddPlane(box, Plane.PositiveX, tileSize);
			AddPlane(box, Plane.PositiveY, tileSize);
			AddPlane(box, Plane.PositiveZ, tileSize);

			AddPlane(box, Plane.NegativeX, tileSize);
			AddPlane(box, Plane.NegativeY, tileSize);
			AddPlane(box, Plane.NegativeZ, tileSize);
		}

		/// <summary>
		/// Method that allows to add a room to the mesh.
		/// As with all rooms it faces inwards.
		/// If you don't want ceiling/floor use <see cref="AddRoomWalls"/>.
		/// </summary>
		/// <param name="box">The bounding box to add to the mesh.</param>
		/// <param name="tileSize">The texture scaling. The scale is applied equaly on all faces.</param>
		public void AddRoom(BoundingBox box, float tileSize = 1.0f)
		{
			AddRoomWalls(box, tileSize);
		}

		/// <summary>
		/// Creates vertices to represent the floor of the given bounding box.
		/// </summary>
		/// <param name="box"></param>
		/// <param name="floorTileSize"></param>
		public void AddFloor(BoundingBox box, float floorTileSize)
		{
			float minX = box.Min.X;
			float minZ = box.Min.Z;
			float y = box.Min.Y;
			float maxX = box.Max.X;
			float maxZ = box.Max.Z;

			AddPlaneXz(minX, maxX, minZ, maxZ, y, floorTileSize);
		}

		/// <summary>
		/// Adds a wall from the bounding box that is 
		/// </summary>
		/// <param name="box"></param>
		/// <param name="plane">The plane which is to be ADDED, it will face in the opposite direction (if you specify <see cref="Plane.NegativeX"/> then this means the "right" plane and it will face to the left).</param>
		/// <param name="tileSize"></param>
		public void AddWall(BoundingBox box, Plane plane, float tileSize = 1.0f)
		{
			Plane opposite;
			switch (plane)
			{
				case Plane.NegativeX:
					opposite = Plane.PositiveX;
					break;
				case Plane.PositiveX:
					opposite = Plane.NegativeX;
					break;
				case Plane.NegativeY:
					opposite = Plane.PositiveY;
					break;
				case Plane.PositiveY:
					opposite = Plane.NegativeY;
					break;
				case Plane.NegativeZ:
					opposite = Plane.PositiveZ;
					break;
				case Plane.PositiveZ:
					opposite = Plane.NegativeZ;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(plane), plane, null);
			}
			AddPlane(box, opposite, tileSize);
		}

		/// <summary>
		/// Adds a plane from the bounding box that is on the specific side, and making it face inwards.
		/// </summary>
		/// <param name="box"></param>
		/// <param name="plane"></param>
		/// <param name="tileSize"></param>
		public void AddPlane(BoundingBox box, Plane plane, float tileSize = 1.0f)
		{
			switch (plane)
			{
				case Plane.NegativeY:
					// Floor
					AddPlaneXz(box.Min.X, box.Max.X, box.Min.Z, box.Max.Z, box.Min.Y, tileSize);
					break;

				case Plane.PositiveY:
					// Ceil
					AddPlaneXz(box.Min.X, box.Max.X, box.Min.Z, box.Max.Z, box.Max.Y, tileSize);
					break;

				case Plane.NegativeZ:
					// wall furthest away from screen
					AddPlaneXy(box.Min.X, box.Max.X, box.Min.Y, box.Max.Y, box.Min.Z, tileSize);
					break;

				case Plane.PositiveZ:
					// wall closest to screen
					AddPlaneXy(box.Min.X, box.Max.X, box.Min.Y, box.Max.Y, box.Max.Z, tileSize);
					break;

				case Plane.NegativeX:
					// wall to the left
					AddPlaneYz(box.Min.Y, box.Max.Y, box.Min.Z, box.Max.Z, box.Min.X, tileSize);
					break;

				case Plane.PositiveX:
					// wall to the right
					AddPlaneYz(box.Min.Y, box.Max.Y, box.Min.Z, box.Max.Z, box.Max.X, tileSize);
					break;

				default:
					throw new ArgumentException();
			}
		}

		public void AddPlaneXy(float minX, float maxX, float minY, float maxY, float z, float wallTileSize)
		{
			float texRepWidth = (maxX - minX) / wallTileSize;
			float texRepHeight = (maxY - minY) / wallTileSize;

			_vertices.Add(new VertexPositionTexture(new Vector3(minX, minY, z), new Vector2(0, texRepHeight)));
			_vertices.Add(new VertexPositionTexture(new Vector3(maxX, minY, z), new Vector2(texRepWidth, texRepHeight)));
			_vertices.Add(new VertexPositionTexture(new Vector3(minX, maxY, z), new Vector2(0, 0)));

			_vertices.Add(new VertexPositionTexture(new Vector3(maxX, minY, z), new Vector2(texRepWidth, texRepHeight)));
			_vertices.Add(new VertexPositionTexture(new Vector3(minX, maxY, z), new Vector2(0, 0)));
			_vertices.Add(new VertexPositionTexture(new Vector3(maxX, maxY, z), new Vector2(texRepWidth, 0)));
		}

		public void AddPlaneXz(float minX, float maxX, float minZ, float maxZ, float y, float floorTileSize)
		{
			float texRepWidth = (maxX - minX) / floorTileSize;
			float texRepDepth = (maxZ - minZ) / floorTileSize;

			_vertices.Add(new VertexPositionTexture(new Vector3(minX, y, minZ), new Vector2(0, 0)));
			_vertices.Add(new VertexPositionTexture(new Vector3(minX, y, maxZ), new Vector2(0, texRepDepth)));
			_vertices.Add(new VertexPositionTexture(new Vector3(maxX, y, minZ), new Vector2(texRepWidth, 0)));

			_vertices.Add(new VertexPositionTexture(new Vector3(minX, y, maxZ), new Vector2(0, texRepDepth)));
			_vertices.Add(new VertexPositionTexture(new Vector3(maxX, y, minZ), new Vector2(texRepWidth, 0)));
			_vertices.Add(new VertexPositionTexture(new Vector3(maxX, y, maxZ), new Vector2(texRepWidth, texRepDepth)));
		}

		public void AddPlaneYz(float minY, float maxY, float minZ, float maxZ, float x, float wallTileSize)
		{
			float texRepWidth = (maxZ - minZ) / wallTileSize;
			float texRepHeight = (maxY - minY) / wallTileSize;

			_vertices.Add(new VertexPositionTexture(new Vector3(x, minY, minZ), new Vector2(0, texRepHeight)));
			_vertices.Add(new VertexPositionTexture(new Vector3(x, minY, maxZ), new Vector2(texRepWidth, texRepHeight)));
			_vertices.Add(new VertexPositionTexture(new Vector3(x, maxY, minZ), new Vector2(0, 0)));

			_vertices.Add(new VertexPositionTexture(new Vector3(x, minY, maxZ), new Vector2(texRepWidth, texRepHeight)));
			_vertices.Add(new VertexPositionTexture(new Vector3(x, maxY, minZ), new Vector2(0, 0)));
			_vertices.Add(new VertexPositionTexture(new Vector3(x, maxY, maxZ), new Vector2(texRepWidth, 0)));
		}

		/// <summary>
		/// Adds walls to the mesh.
		/// The walls are facing inwards like a room.
		/// </summary>
		/// <param name="box"></param>
		/// <param name="tileSize"></param>
		public void AddRoomWalls(BoundingBox box, float tileSize = 1.0f)
		{
			// 4 Walls
			AddPlane(box, Plane.PositiveX, tileSize);
			AddPlane(box, Plane.PositiveZ, tileSize);

			AddPlane(box, Plane.NegativeX, tileSize);
			AddPlane(box, Plane.NegativeZ, tileSize);
		}

		public void Clear()
		{
			_vertices.Clear();
		}

		#endregion
	}
}