using System;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering.Meshes
{
	/// <summary>
	/// Mutable, unindexes mesh.
	/// </summary>
	public abstract class DynamicMesh : Mesh
	{
		#region Methods

		public static int CalcPrimitives(PrimitiveType type, int vertexCount)
		{
			switch (type)
			{
				case PrimitiveType.LineList:
					if (vertexCount == 1)
					{
						return 0;
					}

					if (vertexCount % 2 != 0)
					{
						throw new ArgumentException("LineList requires a vertex-count that is a multiple of 2");
					}

					return vertexCount / 2;

				case PrimitiveType.LineStrip:
					if (vertexCount < 1)
					{
						return 0;
					}

					return vertexCount - 1;

				case PrimitiveType.TriangleList:
					if (vertexCount % 3 != 0)
					{
						throw new ArgumentException("TriangleList requires a vertex-count that is a multiple of 3");
					}

					return vertexCount / 3;

				case PrimitiveType.TriangleStrip:
					if (vertexCount < 3)
					{
						throw new ArgumentException("Not enough triangles, at least 3 required");
					}

					return vertexCount - 2;

				default:
					throw new InvalidOperationException($"Unknown primitive type: {type}");
			}
		}

		public abstract void Update<T>(T[] vertices)
			where T : struct, IVertexType;

		public abstract void Update<T>(T[] vertices, PrimitiveType type)
			where T : struct, IVertexType;

		#endregion
	}
}