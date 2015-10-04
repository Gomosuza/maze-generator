using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering.Meshes
{
	public abstract class Mesh
	{
		#region Properties

		/// <summary>
		/// Number of primitives.
		/// </summary>
		public abstract int Primitives { get; }

		/// <summary>
		/// Type of primitives.
		/// </summary>
		public abstract PrimitiveType Type { get; }

		/// <summary>
		/// Number of vertices.
		/// </summary>
		public abstract int Vertices { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Attaches this mesh to the given device.
		/// </summary>
		internal abstract void Attach();

		internal abstract void Detach();

		internal abstract void Draw();

		#endregion
	}
}