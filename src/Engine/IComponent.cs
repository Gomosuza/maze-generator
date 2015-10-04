using Microsoft.Xna.Framework;
using Engine.Input;
using Engine.Rendering;

namespace Engine
{
	public interface IComponent
	{
		#region Methods

		void Render(IRenderContext renderContext, GameTime dt);

		void Update(KeyboardManager keyboard, MouseManager mouse, GameTime dt);

		#endregion
	}
}