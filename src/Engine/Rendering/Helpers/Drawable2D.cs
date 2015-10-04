using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering.Helpers
{
	/// <summary>
	/// Helper for storing 2D draw calls.
	/// </summary>
	public abstract class Drawable2D
	{
		#region Constructors

		protected Drawable2D(float layerDepth)
		{
			LayerDepth = layerDepth;
		}

		#endregion

		#region Properties

		public float LayerDepth { get; private set; }

		#endregion

		#region Methods

		public abstract void Draw(SpriteBatch spriteBatch);

		#endregion
	}
}