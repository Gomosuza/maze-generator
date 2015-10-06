using Microsoft.Xna.Framework;
using Engine.Rendering.Meshes;

namespace Engine.Rendering
{
	/// <summary>
	/// The 3D rendercontext is responsible for rendering 3D content such as meshes and models.
	/// </summary>
	public interface IRenderContext3D
	{
		#region Properties

		/// <summary>
		/// The camera attached to this render context.
		/// Must be set before the first render pass.
		/// Can be swapped out at any time, however it is recommend to only change it once before rendering as swapping camera mid-rendering will often
		/// result in confusing results.
		/// </summary>
		ICamera Camera { get; set; }

		/// <summary>
		/// The render context to which this 3D renderer is attached to.
		/// </summary>
		IRenderContext RenderContext { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Draws a mesh using the provided pen and brush.
		/// If none are provided, the renderer implementation may offer a default implementation.
		/// </summary>
		/// <param name="mesh"></param>
		/// <param name="world"></param>
		/// <param name="brush"></param>
		/// <param name="pen"></param>
		void DrawMesh(Mesh mesh, Matrix world, Brush brush = null, Pen pen = null);

		#endregion
	}
}