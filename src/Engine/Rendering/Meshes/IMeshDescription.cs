using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering.Meshes
{
	public interface IMeshDescription
	{
		#region Properties

		PrimitiveType PrimitiveType { get; }

		int VertexCount { get; }

		VertexPositionTexture[] Vertices { get; }

		#endregion
	}
}