using System;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering.Meshes
{
	/// <summary>
	/// This dynamic mesh implementation uses a circular queue to append vertex data to its buffer instead of replacing it.
	/// Is is very well suited for meshes that get drawn and updated MULTIPLE times PER frame in a jagged-kind of fashion (Update1, Draw1, Update2, Draw2, ..).
	/// 
	/// Fix for common issue: http://blogs.msdn.com/b/shawnhar/archive/2010/07/07/setdataoptions-nooverwrite-versus-discard.aspx
	/// </summary>
	internal sealed class AppendingMesh : DynamicMesh
	{
		#region Fields

		private readonly VertexDeclaration _declaration;
		private readonly GraphicsDevice _device;
		private readonly Type _vertexType;

		private int _numPrimitives;
		private int _numVertices;
		private PrimitiveType _type;
		private DynamicVertexBuffer _vertexBuffer;
		private int _verticesStartIndex;

		#endregion

		#region Constructors

		public AppendingMesh(GraphicsDevice device, Type vertexType, VertexDeclaration decl, PrimitiveType type)
		{
			_device = device;
			_vertexType = vertexType;
			_declaration = decl;
			_type = type;
		}

		#endregion

		#region Properties

		public override int Primitives
		{
			get { return _numPrimitives; }
		}

		public override PrimitiveType Type
		{
			get { return _type; }
		}

		public override int Vertices
		{
			get { return _numVertices; }
		}

		private int VerticesEndIndex
		{
			get { return _verticesStartIndex + _numVertices; }
		}

		#endregion

		#region Methods

		public override void Update<T>(T[] vertices)
		{
			Update(vertices, _type);
		}

		public override void Update<T>(T[] vertices, PrimitiveType type)
		{
			if (typeof(T) != _vertexType)
			{
				throw new ArgumentException();
			}

			_type = type;
			Append(vertices);
		}

		internal override void Attach()
		{
			_vertexBuffer.GraphicsDevice.SetVertexBuffer(_vertexBuffer);
		}

		internal override void Detach()
		{
			_vertexBuffer.GraphicsDevice.SetVertexBuffer(null);
		}

		internal override void Draw()
		{
			_vertexBuffer.GraphicsDevice.DrawPrimitives(_type, _verticesStartIndex, _numPrimitives);
		}

		private void Append<T>(T[] vertices)
			where T : struct
		{
			int numPrimitives = CalcPrimitives(_type, vertices.Length);

			int vertexLength = vertices.Length;
			const int growSize = 1024;
			const int clearSize = 10 * growSize;

			if (_vertexBuffer == null || VerticesEndIndex + vertexLength > _vertexBuffer.VertexCount)
			{
				int minVertexSize = VerticesEndIndex + vertexLength;
				int newVertexSize = (int)(Math.Ceiling(1.0 * minVertexSize / growSize) * growSize);

				_vertexBuffer = new DynamicVertexBuffer(_device, _declaration, newVertexSize, BufferUsage.WriteOnly);
				_verticesStartIndex = 0;
				_numVertices = 0;
			}

			if (VerticesEndIndex + vertexLength > clearSize)
			{
				// Begin at start...
				_vertexBuffer.SetData(vertices, 0, vertexLength, SetDataOptions.Discard);
				_verticesStartIndex = 0;
				_numVertices = vertexLength;
			}
			else
			{
				// Append mode
				_vertexBuffer.SetData(VerticesEndIndex * _declaration.VertexStride, //< where to start in the VERTEX BUFFER
					vertices, //< what to copy
					0, //< start index in the ARRAY
					vertexLength, //< length of the ARRAY
					_declaration.VertexStride, //< size of ONE vertex
					SetDataOptions.NoOverwrite);

				_verticesStartIndex += _numVertices;
				_numVertices = vertices.Length;
			}

			_numPrimitives = numPrimitives;
		}

		#endregion
	}
}