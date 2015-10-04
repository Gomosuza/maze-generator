using Microsoft.Xna.Framework;
using Engine.Input;
using Engine.Rendering;

namespace Engine
{
	/// <summary>
	/// Base interface for any component of the game.
	/// Allows updating and rendering.
	/// </summary>
	public interface IComponent
	{
		#region Methods

		/// <summary>
		/// Render is called each frame to allow drawing using the provided render context.
		/// </summary>
		/// <param name="renderContext"></param>
		/// <param name="dt"></param>
		void Render(IRenderContext renderContext, GameTime dt);

		/// <summary>
		/// Update is also called each frame to allow input processing and logic updates.
		/// </summary>
		/// <param name="keyboard"></param>
		/// <param name="mouse"></param>
		/// <param name="dt"></param>
		void Update(KeyboardManager keyboard, MouseManager mouse, GameTime dt);

		#endregion
	}
}