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
	/// </summary>
	public sealed class MeshDescriptionBuilder : IMeshDescription
	{
		#region Fields

		private readonly List<VertexPositionTexture> _vertices;

		#endregion

		#region Constructors

		public MeshDescriptionBuilder()
		{
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

		public void AddBox(BoundingBox bbox, float tileSize = 1.0f)
		{
			AddPlane(bbox, Plane.PositiveX, tileSize);
			AddPlane(bbox, Plane.PositiveY, tileSize);
			AddPlane(bbox, Plane.PositiveZ, tileSize);

			AddPlane(bbox, Plane.NegativeX, tileSize);
			AddPlane(bbox, Plane.NegativeY, tileSize);
			AddPlane(bbox, Plane.NegativeZ, tileSize);
		}

		/// <summary>
		/// Creates vertices to represent the floor of the given bounding box in as a trianglestrip primitive.
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

		public void AddPlane(BoundingBox bbox, Plane plane, float tileSize = 1.0f)
		{
			switch (plane)
			{
				case Plane.NegativeY:
					// Floor
					AddPlaneXz(bbox.Min.X, bbox.Max.X, bbox.Min.Z, bbox.Max.Z, bbox.Min.Y, tileSize);
					break;

				case Plane.PositiveY:
					// Ceil
					AddPlaneXz(bbox.Min.X, bbox.Max.X, bbox.Min.Z, bbox.Max.Z, bbox.Max.Y, tileSize);
					break;

				case Plane.NegativeZ:
					AddPlaneXy(bbox.Min.X, bbox.Max.X, bbox.Min.Y, bbox.Max.Y, bbox.Min.Z, tileSize);
					break;

				case Plane.PositiveZ:
					AddPlaneXy(bbox.Min.X, bbox.Max.X, bbox.Min.Y, bbox.Max.Y, bbox.Max.Z, tileSize);
					break;

				case Plane.NegativeX:
					AddPlaneYz(bbox.Min.Y, bbox.Max.Y, bbox.Min.Z, bbox.Max.Z, bbox.Min.X, tileSize);
					break;

				case Plane.PositiveX:
					AddPlaneYz(bbox.Min.Y, bbox.Max.Y, bbox.Min.Z, bbox.Max.Z, bbox.Max.X, tileSize);
					break;

				default:
					throw new ArgumentException();
			}
		}

		public void AddPlaneXy(float minX, float maxX, float minY, float maxY, float z, float wallTileSize)
		{
			float texRepWidth = (maxX - minX) / wallTileSize;
			float texRepHeight = (maxY - minY) / wallTileSize;

			switch (PrimitiveType)
			{
				case PrimitiveType.TriangleList:

					_vertices.Add(new VertexPositionTexture(new Vector3(minX, minY, z), new Vector2(0, texRepHeight)));
					_vertices.Add(new VertexPositionTexture(new Vector3(maxX, minY, z), new Vector2(texRepWidth, texRepHeight)));
					_vertices.Add(new VertexPositionTexture(new Vector3(minX, maxY, z), new Vector2(0, 0)));

					_vertices.Add(new VertexPositionTexture(new Vector3(maxX, minY, z), new Vector2(texRepWidth, texRepHeight)));
					_vertices.Add(new VertexPositionTexture(new Vector3(minX, maxY, z), new Vector2(0, 0)));
					_vertices.Add(new VertexPositionTexture(new Vector3(maxX, maxY, z), new Vector2(texRepWidth, 0)));

					break;

				case PrimitiveType.LineList:

					// #1
					_vertices.Add(new VertexPositionTexture(new Vector3(minX, minY, z), new Vector2(0, 0)));
					_vertices.Add(new VertexPositionTexture(new Vector3(maxX, minY, z), new Vector2(0, texRepHeight)));

					// #2
					_vertices.Add(new VertexPositionTexture(new Vector3(maxX, minY, z), new Vector2(0, texRepHeight)));
					_vertices.Add(new VertexPositionTexture(new Vector3(maxX, maxY, z), new Vector2(texRepWidth, texRepHeight)));

					// #3
					_vertices.Add(new VertexPositionTexture(new Vector3(maxX, maxY, z), new Vector2(texRepWidth, texRepHeight)));
					_vertices.Add(new VertexPositionTexture(new Vector3(minX, maxY, z), new Vector2(texRepWidth, 0)));

					// #4
					_vertices.Add(new VertexPositionTexture(new Vector3(minX, maxY, z), new Vector2(texRepWidth, 0)));
					_vertices.Add(new VertexPositionTexture(new Vector3(minX, minY, z), new Vector2(0, 0)));

					break;

				case PrimitiveType.LineStrip:

					// 5 vertices, 4 primitives
					_vertices.Add(new VertexPositionTexture(new Vector3(minX, minY, z), new Vector2(0, 0)));
					_vertices.Add(new VertexPositionTexture(new Vector3(maxX, minY, z), new Vector2(0, texRepHeight)));
					_vertices.Add(new VertexPositionTexture(new Vector3(maxX, maxY, z), new Vector2(texRepWidth, texRepHeight)));
					_vertices.Add(new VertexPositionTexture(new Vector3(minX, maxY, z), new Vector2(texRepWidth, 0)));
					_vertices.Add(new VertexPositionTexture(new Vector3(minX, minY, z), new Vector2(0, 0)));

					break;

				default:

					throw new InvalidOperationException(string.Format("{0} is not supported", PrimitiveType));
			}
		}

		public void AddPlaneXz(float minX, float maxX, float minZ, float maxZ, float y, float floorTileSize)
		{
			float texRepWidth = (maxX - minX) / floorTileSize;
			float texRepDepth = (maxZ - minZ) / floorTileSize;

			switch (PrimitiveType)
			{
				case PrimitiveType.TriangleList:

					_vertices.Add(new VertexPositionTexture(new Vector3(minX, y, minZ), new Vector2(0, 0)));
					_vertices.Add(new VertexPositionTexture(new Vector3(minX, y, maxZ), new Vector2(0, texRepDepth)));
					_vertices.Add(new VertexPositionTexture(new Vector3(maxX, y, minZ), new Vector2(texRepWidth, 0)));

					_vertices.Add(new VertexPositionTexture(new Vector3(minX, y, maxZ), new Vector2(0, texRepDepth)));
					_vertices.Add(new VertexPositionTexture(new Vector3(maxX, y, minZ), new Vector2(texRepWidth, 0)));
					_vertices.Add(new VertexPositionTexture(new Vector3(maxX, y, maxZ), new Vector2(texRepWidth, texRepDepth)));

					break;

				case PrimitiveType.LineList:

					// #1
					_vertices.Add(new VertexPositionTexture(new Vector3(minX, y, minZ), new Vector2(0, 0)));
					_vertices.Add(new VertexPositionTexture(new Vector3(minX, y, maxZ), new Vector2(0, texRepDepth)));

					// #2
					_vertices.Add(new VertexPositionTexture(new Vector3(minX, y, maxZ), new Vector2(0, texRepDepth)));
					_vertices.Add(new VertexPositionTexture(new Vector3(maxX, y, maxZ), new Vector2(texRepWidth, texRepDepth)));

					// #3
					_vertices.Add(new VertexPositionTexture(new Vector3(maxX, y, maxZ), new Vector2(texRepWidth, texRepDepth)));
					_vertices.Add(new VertexPositionTexture(new Vector3(maxX, y, minZ), new Vector2(texRepWidth, 0)));

					// #4
					_vertices.Add(new VertexPositionTexture(new Vector3(maxX, y, minZ), new Vector2(texRepWidth, 0)));
					_vertices.Add(new VertexPositionTexture(new Vector3(minX, y, minZ), new Vector2(0, 0)));

					break;

				case PrimitiveType.LineStrip:

					// 5 vertices, 4 primitives
					_vertices.Add(new VertexPositionTexture(new Vector3(minX, y, minZ), new Vector2(0, 0)));
					_vertices.Add(new VertexPositionTexture(new Vector3(minX, y, maxZ), new Vector2(0, texRepDepth)));
					_vertices.Add(new VertexPositionTexture(new Vector3(maxX, y, maxZ), new Vector2(texRepWidth, texRepDepth)));
					_vertices.Add(new VertexPositionTexture(new Vector3(maxX, y, minZ), new Vector2(texRepWidth, 0)));
					_vertices.Add(new VertexPositionTexture(new Vector3(minX, y, minZ), new Vector2(0, 0)));

					break;

				default:

					throw new InvalidOperationException($"{PrimitiveType} is not supported");
			}
		}

		public void AddPlaneYz(float minY, float maxY, float minZ, float maxZ, float x, float wallTileSize)
		{
			float texRepWidth = (maxZ - minZ) / wallTileSize;
			float texRepHeight = (maxY - minY) / wallTileSize;

			switch (PrimitiveType)
			{
				case PrimitiveType.TriangleList:

					_vertices.Add(new VertexPositionTexture(new Vector3(x, minY, minZ), new Vector2(0, texRepHeight)));
					_vertices.Add(new VertexPositionTexture(new Vector3(x, minY, maxZ), new Vector2(texRepWidth, texRepHeight)));
					_vertices.Add(new VertexPositionTexture(new Vector3(x, maxY, minZ), new Vector2(0, 0)));

					_vertices.Add(new VertexPositionTexture(new Vector3(x, minY, maxZ), new Vector2(texRepWidth, texRepHeight)));
					_vertices.Add(new VertexPositionTexture(new Vector3(x, maxY, minZ), new Vector2(0, 0)));
					_vertices.Add(new VertexPositionTexture(new Vector3(x, maxY, maxZ), new Vector2(texRepWidth, 0)));

					break;

				case PrimitiveType.LineList:

					// #1
					_vertices.Add(new VertexPositionTexture(new Vector3(x, minY, minZ), new Vector2(0, 0)));
					_vertices.Add(new VertexPositionTexture(new Vector3(x, minY, maxZ), new Vector2(0, texRepHeight)));

					// #2
					_vertices.Add(new VertexPositionTexture(new Vector3(x, minY, maxZ), new Vector2(0, texRepHeight)));
					_vertices.Add(new VertexPositionTexture(new Vector3(x, maxY, maxZ), new Vector2(texRepWidth, texRepHeight)));

					// #3
					_vertices.Add(new VertexPositionTexture(new Vector3(x, maxY, maxZ), new Vector2(texRepWidth, texRepHeight)));
					_vertices.Add(new VertexPositionTexture(new Vector3(x, maxY, minZ), new Vector2(texRepWidth, 0)));

					// #4
					_vertices.Add(new VertexPositionTexture(new Vector3(x, maxY, minZ), new Vector2(texRepWidth, 0)));
					_vertices.Add(new VertexPositionTexture(new Vector3(x, minY, minZ), new Vector2(0, 0)));

					break;

				case PrimitiveType.LineStrip:

					// 5 vertices, 4 primitives
					_vertices.Add(new VertexPositionTexture(new Vector3(x, minY, minZ), new Vector2(0, 0)));
					_vertices.Add(new VertexPositionTexture(new Vector3(x, minY, maxZ), new Vector2(0, texRepHeight)));
					_vertices.Add(new VertexPositionTexture(new Vector3(x, maxY, maxZ), new Vector2(texRepWidth, texRepHeight)));
					_vertices.Add(new VertexPositionTexture(new Vector3(x, maxY, minZ), new Vector2(texRepWidth, 0)));
					_vertices.Add(new VertexPositionTexture(new Vector3(x, minY, minZ), new Vector2(0, 0)));

					break;

				default:

					throw new InvalidOperationException($"{PrimitiveType} is not supported");
			}
		}

		public void AddWalls(BoundingBox bbox, float tileSize = 1.0f)
		{
			// 4 Walls
			AddPlane(bbox, Plane.PositiveX, tileSize);
			AddPlane(bbox, Plane.PositiveZ, tileSize);

			AddPlane(bbox, Plane.NegativeX, tileSize);
			AddPlane(bbox, Plane.NegativeZ, tileSize);
		}

		public void Clear()
		{
			_vertices.Clear();
		}

		#endregion
	}
}