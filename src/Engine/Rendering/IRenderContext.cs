using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Engine.Rendering.Meshes;

namespace Engine.Rendering
{
	/// <summary>
	/// The rendercontext holds all information required for rendering.
	/// If you mix 2D and 3D graphics you need to be aware that the renderer will make two passes: first it draws all 3D, then all 2D on top.
	/// If for some reason you need immediate rendering (so some 3D stuff ends up on top of 2D stuff) then you need to use multiple render contexts and sort them yourself.
	/// </summary>
	public interface IRenderContext
	{
		#region Properties

		ContentManager Content { get; }

		/// <summary>
		/// The attached graphics device.
		/// </summary>
		GraphicsDevice GraphicsDevice { get; }

		/// <summary>
		/// The attached graphics device manager.
		/// </summary>
		GraphicsDeviceManager GraphicsDeviceManager { get; }

		/// <summary>
		/// Allows creation of meshes
		/// </summary>
		IMeshCreator MeshCreator { get; }

		/// <summary>
		/// The 2D renderer attached to this rendercontext.
		/// </summary>
		IRenderContext2D RenderContext2D { get; }

		/// <summary>
		/// The 3D renderer attached to this rendercontext.
		/// </summary>
		IRenderContext3D RenderContext3D { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Must be called before any rendering can occur.
		/// </summary>
		void Attach();

		/// <summary>
		/// Must be called after all rendering calls have been made.
		/// This will either flush all remaining draw calls or clear the pipeline.
		/// </summary>
		void Detach();

		#endregion
	}
}