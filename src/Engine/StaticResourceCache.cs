using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
	public class StaticResourceCache
	{
		#region Fields

		private static Texture2D _white;

		#endregion

		#region Methods

		public static Texture2D GetWhite(GraphicsDevice device)
		{
			if (_white == null)
			{
				_white = new Texture2D(device, 1, 1);
				_white.SetData(new[] {Color.White});
			}
			return _white;
		}

		#endregion
	}
}