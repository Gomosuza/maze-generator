using System;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering.Meshes
{
	public class MeshCreator : IMeshCreator
	{
		#region Fields

		private readonly GraphicsDevice _device;

		#endregion

		#region Constructors

		public MeshCreator(GraphicsDevice device)
		{
			_device = device;
		}

		#endregion

		#region Methods

		public DynamicMesh CreateDynamicMesh(PrimitiveType type, Type vertexType, VertexDeclaration declaration, DynamicMeshUsage usage)
		{
			// according to shawnhar (http://xboxforums.create.msdn.com/forums/t/5136.aspx)
			//
			// ''For persistent geometry (data that is drawn many times without changing), DrawPrimitives with a vertex buffer is dramatically faster.
			// For dynamic geometry(which is created on the fly each time it is drawn), you should use DrawUserPrimitives, however.
			// This is able to do the copy in a more efficient way than if you manually created a vertex buffer, SetData into it, and then drew from that.''
			//
			// However for DynamicMeshUsage.UpdateOften we use our appendingmesh which uses circlular buffer/appends the data to the buffer

			switch (usage)
			{
				case DynamicMeshUsage.UpdateOften:
					// TODO: do performance comparison against shawnhars suggested DrawUserPrimitives method
					return new AppendingMesh(_device, vertexType, declaration, type);
				case DynamicMeshUsage.UpdateSeldom:
					return new UpdatableDynamicMesh(_device, type);
				default:
					throw new ArgumentOutOfRangeException(nameof(usage), usage, null);
			}
		}

		public DynamicMesh CreateDynamicMesh<T>(PrimitiveType type, T[] vertices, DynamicMeshUsage usage)
			where T : struct, IVertexType
		{
			if (vertices == null)
			{
				throw new ArgumentNullException();
			}
			if (vertices.Length == 0)
			{
				throw new ArgumentException();
			}

			var vertexType = typeof(T);
			var decl = vertices[0].VertexDeclaration;

			var mesh = CreateDynamicMesh(type, vertexType, decl, usage);
			mesh.Update(vertices);
			return mesh;
		}

		public DynamicMesh CreateDynamicMesh(IMeshDescription description, DynamicMeshUsage usage)
		{
			if (description == null)
			{
				throw new ArgumentNullException(nameof(description));
			}

			return CreateDynamicMesh(description.PrimitiveType, description.Vertices, usage);
		}

		public Mesh CreateMesh<T>(PrimitiveType type, T[] vertices)
			where T : struct, IVertexType
		{
			var mesh = new StaticMesh<T>(_device, type, vertices);
			return mesh;
		}

		public Mesh CreateMesh(IMeshDescription description)
		{
			return CreateMesh(description.PrimitiveType, description.Vertices);
		}

		#endregion
	}
}