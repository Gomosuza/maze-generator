using System;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering.Meshes
{
	internal class StaticMesh<T> : Mesh
		where T : struct, IVertexType
	{
		#region Fields

		private readonly VertexBuffer _buffer;
		private readonly GraphicsDevice _device;
		private readonly int _primitives;
		private readonly PrimitiveType _type;

		#endregion

		#region Constructors

		public StaticMesh(GraphicsDevice device, PrimitiveType type, T[] vertices)
		{
			_type = type;
			_device = device;

			if (vertices == null)
			{
				throw new ArgumentNullException(nameof(vertices));
			}

			if (vertices.Length == 0)
			{
				_primitives = 0;
				_buffer = null;
			}
			else
			{
				// Throws in case the primitive count is off...
				_primitives = DynamicMesh.CalcPrimitives(type, vertices.Length);

				var decl = vertices[0].VertexDeclaration;

				_buffer = new VertexBuffer(_device, decl, vertices.Length, BufferUsage.WriteOnly);
				_buffer.SetData(vertices);
			}
		}

		#endregion

		#region Properties

		public override int Primitives
		{
			get { return _primitives; }
		}

		public override PrimitiveType Type
		{
			get { return _type; }
		}

		public override int Vertices
		{
			get { return _buffer?.VertexCount ?? 0; }
		}

		#endregion

		#region Methods

		internal override void Attach()
		{
			if (_primitives == 0)
			{
				return;
			}

			_device.SetVertexBuffer(_buffer);
		}

		internal override void Detach()
		{
			_device.SetVertexBuffer(null);
		}

		internal override void Draw()
		{
			if (_primitives == 0)
			{
				return;
			}

			_device.DrawPrimitives(_type, 0, _primitives);
		}

		#endregion
	}
}