using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering
{
	/// <summary>
	/// The 3D rendercontext is responsible for rendering 2D content
	/// </summary>
	public interface IRenderContext2D
	{
		#region Properties

		/// <summary>
		/// The render context to which this 2D renderer is attached to.
		/// </summary>
		IRenderContext RenderContext { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Draws the given text at the given position.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="position"></param>
		/// <param name="color"></param>
		/// <param name="size"></param>
		/// <param name="layerDepth"></param>
		void DrawString(string text, Vector2 position, Color color, FontSize size = FontSize.Medium, float layerDepth = 0f);

		/// <summary>
		/// Draws a texture at the given location
		/// </summary>
		/// <param name="texture">The texture to draw.</param>
		/// <param name="position">The position to draw at in pixels.</param>
		/// <param name="origin">The origin of the texture to use.</param>
		/// <param name="scale">The scale of the texture to draw.</param>
		/// <param name="layerDepth"></param>
		void DrawTexture(Texture2D texture, Vector2 position, Vector2 origin, float scale = 1f, float layerDepth = 0);

		/// <summary>
		/// Draws a texture at the given location
		/// </summary>
		/// <param name="texture">The texture to draw.</param>
		/// <param name="position">The position to draw at in pixels.</param>
		/// <param name="origin">The origin of the texture to use.</param>
		/// <param name="scale">The scale of the texture to draw.</param>
		/// <param name="color">The color to use. Will be multiplied with the texture.</param>
		/// <param name="layerDepth"></param>
		void DrawTexture(Texture2D texture, Vector2 position, Vector2 origin, float scale, Color color, float layerDepth = 0);

		#endregion
	}
}