using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering.Meshes
{
	/// <summary>
	/// The mesh description can be fed to the <see cref="IMeshCreator"/> which will then create a proper mesh from it.
	/// </summary>
	public interface IMeshDescription
	{
		#region Properties

		/// <summary>
		/// The primitive type in use.
		/// </summary>
		PrimitiveType PrimitiveType { get; }

		/// <summary>
		/// The total vertex count.
		/// </summary>
		int VertexCount { get; }

		/// <summary>
		/// The actual vertices.
		/// </summary>
		VertexPositionTexture[] Vertices { get; }

		#endregion
	}
}