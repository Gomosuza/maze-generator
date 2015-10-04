using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering
{
	/// <summary>
	/// Describes a material that defines the visual aspects of an object (mostly meshes).
	/// There can be several kinds of materials.
	/// </summary>
	public abstract class Brush
	{
		#region Properties

		/// <summary>
		/// Whether or not this brush is already prepared for drawing.
		/// </summary>
		internal abstract bool IsPrepared { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Configures the given effect to use this brush.
		/// </summary>
		/// <param name="effect"></param>
		internal abstract void Configure(BasicEffect effect);

		/// <summary>
		/// Prepares this brush for drawing.
		/// Wil be called before rendering it if <see cref="IsPrepared"/> returned false.
		/// </summary>
		internal abstract void Prepare(IRenderContext renderContext);

		#endregion
	}
}