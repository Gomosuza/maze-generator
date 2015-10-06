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
	/// This type will always use <see cref="PrimitiveType.TriangleList"/> and <see cref="VertexPositionTexture"/>.
	/// </summary>
	public sealed class TexturedMeshDescriptionBuilder : IMeshDescription
	{
		#region Fields

		private readonly List<VertexPositionTexture> _vertices;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new mesh description builder.
		/// This type will always use <see cref="PrimitiveType.TriangleList"/>.
		/// </summary>
		public TexturedMeshDescriptionBuilder()
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
			AddPlane(box, Plane.PositiveX, true, tileSize);
			AddPlane(box, Plane.PositiveY, true, tileSize);
			AddPlane(box, Plane.PositiveZ, true, tileSize);

			AddPlane(box, Plane.NegativeX, true, tileSize);
			AddPlane(box, Plane.NegativeY, true, tileSize);
			AddPlane(box, Plane.NegativeZ, true, tileSize);
		}

		/// <summary>
		/// Creates vertices to represent the floor of the given bounding box.
		/// </summary>
		/// <param name="box"></param>
		/// <param name="floorTileSize"></param>
		public void AddFloor(BoundingBox box, float floorTileSize)
		{
			AddPlane(box, Plane.NegativeY, true, floorTileSize);
		}

		/// <summary>
		/// Adds a plane from the bounding box that is on the specific side, and making it face inwards.
		/// </summary>
		/// <param name="box"></param>
		/// <param name="side">The side of the cube to draw.</param>
		/// <param name="faceOutwards">If true, will render the outside plane.</param>
		/// <param name="tileSize"></param>
		public void AddPlane(BoundingBox box, Plane side, bool faceOutwards = true, float tileSize = 1.0f)
		{
			switch (side)
			{
				case Plane.NegativeY:
					// Floor
					AddPlaneXz(box.Min.X, box.Max.X, box.Min.Z, box.Max.Z, box.Min.Y, !faceOutwards, tileSize);
					break;

				case Plane.PositiveY:
					// Ceil
					AddPlaneXz(box.Min.X, box.Max.X, box.Min.Z, box.Max.Z, box.Max.Y, faceOutwards, tileSize);
					break;

				case Plane.NegativeZ:
					// wall furthest away from screen
					AddPlaneXy(box.Min.X, box.Max.X, box.Min.Y, box.Max.Y, box.Min.Z, !faceOutwards, tileSize);
					break;

				case Plane.PositiveZ:
					// wall closest to screen
					AddPlaneXy(box.Min.X, box.Max.X, box.Min.Y, box.Max.Y, box.Max.Z, faceOutwards, tileSize);
					break;

				case Plane.NegativeX:
					// wall to the left
					AddPlaneYz(box.Min.Y, box.Max.Y, box.Min.Z, box.Max.Z, box.Min.X, !faceOutwards, tileSize);
					break;

				case Plane.PositiveX:
					// wall to the right
					AddPlaneYz(box.Min.Y, box.Max.Y, box.Min.Z, box.Max.Z, box.Max.X, faceOutwards, tileSize);
					break;

				default:
					throw new ArgumentException();
			}
		}

		/// <summary>
		/// Creates a new plane on the XY axis.
		/// </summary>
		/// <param name="minX"></param>
		/// <param name="maxX"></param>
		/// <param name="minY"></param>
		/// <param name="maxY"></param>
		/// <param name="z"></param>
		/// <param name="faceNegativeAxis">If true, will face in negative Z direction, otherwise in positive Z direction.</param>
		/// <param name="tileSize"></param>
		public void AddPlaneXy(float minX, float maxX, float minY, float maxY, float z, bool faceNegativeAxis, float tileSize)
		{
			float texRepWidth = (maxX - minX) / tileSize;
			float texRepHeight = (maxY - minY) / tileSize;

			var vertices = new List<VertexPositionTexture>
			{
				new VertexPositionTexture(new Vector3(minX, maxY, z), new Vector2(0, 0)),
				new VertexPositionTexture(new Vector3(maxX, minY, z), new Vector2(texRepWidth, texRepHeight)),
				new VertexPositionTexture(new Vector3(minX, minY, z), new Vector2(0, texRepHeight)),
				new VertexPositionTexture(new Vector3(maxX, minY, z), new Vector2(texRepWidth, texRepHeight)),
				new VertexPositionTexture(new Vector3(minX, maxY, z), new Vector2(0, 0)),
				new VertexPositionTexture(new Vector3(maxX, maxY, z), new Vector2(texRepWidth, 0))
			};

			if (!faceNegativeAxis)
			{
				vertices.Reverse();
			}
			_vertices.AddRange(vertices);
		}

		/// <summary>
		/// Creates a new plane on the XZ axis.
		/// </summary>
		/// <param name="minX"></param>
		/// <param name="maxX"></param>
		/// <param name="minZ"></param>
		/// <param name="maxZ"></param>
		/// <param name="y"></param>
		/// <param name="faceNegativeAxis">If true, will face in negative Y direction, otherwise in positive Y direction.</param>
		/// <param name="tileSize"></param>
		public void AddPlaneXz(float minX, float maxX, float minZ, float maxZ, float y, bool faceNegativeAxis, float tileSize)
		{
			float texRepWidth = (maxX - minX) / tileSize;
			float texRepDepth = (maxZ - minZ) / tileSize;

			var vertices = new List<VertexPositionTexture>
			{
				new VertexPositionTexture(new Vector3(maxX, y, minZ), new Vector2(texRepWidth, 0)),
				new VertexPositionTexture(new Vector3(minX, y, maxZ), new Vector2(0, texRepDepth)),
				new VertexPositionTexture(new Vector3(minX, y, minZ), new Vector2(0, 0)),
				new VertexPositionTexture(new Vector3(minX, y, maxZ), new Vector2(0, texRepDepth)),
				new VertexPositionTexture(new Vector3(maxX, y, minZ), new Vector2(texRepWidth, 0)),
				new VertexPositionTexture(new Vector3(maxX, y, maxZ), new Vector2(texRepWidth, texRepDepth))
			};

			if (!faceNegativeAxis)
			{
				vertices.Reverse();
			}
			_vertices.AddRange(vertices);
		}

		/// <summary>
		/// Creates a new plane on the YZ axis.
		/// </summary>
		/// <param name="minY"></param>
		/// <param name="maxY"></param>
		/// <param name="minZ"></param>
		/// <param name="maxZ"></param>
		/// <param name="x"></param>
		/// <param name="faceNegativeAxis">If true, will face in negative X direction, otherwise in positive X direction.</param>
		/// <param name="tileSize"></param>
		public void AddPlaneYz(float minY, float maxY, float minZ, float maxZ, float x, bool faceNegativeAxis, float tileSize)
		{
			float texRepWidth = (maxZ - minZ) / tileSize;
			float texRepHeight = (maxY - minY) / tileSize;

			var vertices = new List<VertexPositionTexture>
			{
				new VertexPositionTexture(new Vector3(x, maxY, minZ), new Vector2(0, 0)),
				new VertexPositionTexture(new Vector3(x, minY, minZ), new Vector2(0, texRepHeight)),
				new VertexPositionTexture(new Vector3(x, minY, maxZ), new Vector2(texRepWidth, texRepHeight)),
				new VertexPositionTexture(new Vector3(x, maxY, maxZ), new Vector2(texRepWidth, 0)),
				new VertexPositionTexture(new Vector3(x, maxY, minZ), new Vector2(0, 0)),
				new VertexPositionTexture(new Vector3(x, minY, maxZ), new Vector2(texRepWidth, texRepHeight))
			};

			if (!faceNegativeAxis)
			{
				vertices.Reverse();
			}
			_vertices.AddRange(vertices);
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
			//AddFloor(box, tileSize);
			AddCeiling(box, tileSize);
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
			AddPlane(box, Plane.PositiveX, false, tileSize);
			AddPlane(box, Plane.PositiveZ, false, tileSize);

			AddPlane(box, Plane.NegativeX, false, tileSize);
			AddPlane(box, Plane.NegativeZ, false, tileSize);
		}

		/// <summary>
		/// Adds a wall from the bounding box that is facing inwards.
		/// </summary>
		/// <param name="box"></param>
		/// <param name="plane"></param>
		/// <param name="tileSize"></param>
		public void AddWall(BoundingBox box, Plane plane, float tileSize = 1.0f)
		{
			AddPlane(box, plane, false, tileSize);
		}

		public void Clear()
		{
			_vertices.Clear();
		}

		private void AddCeiling(BoundingBox box, float tileSize)
		{
			AddPlane(box, Plane.PositiveY, false, tileSize);
		}

		#endregion
	}
}