using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering.Helpers
{
	/// <summary>
	/// Helper to store 2D font draw calls to later draw them all in one batch.
	/// </summary>
	public class FontEntity2D : Drawable2D
	{
		#region Fields

		private readonly Color _color;
		private readonly SpriteEffects _effects;
		private readonly SpriteFont _font;
		private readonly Vector2 _origin;
		private readonly Vector2 _positon;
		private readonly float _rotation;
		private readonly Vector2 _scale;
		private readonly string _text;

		#endregion

		#region Constructors

		public FontEntity2D(SpriteFont font, string text, Vector2 positon, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
			: base(layerDepth)
		{
			_font = font;
			_text = text;
			_positon = positon;
			_color = color;
			_rotation = rotation;
			_origin = origin;
			_scale = scale;
			_effects = effects;
		}

		#endregion

		#region Methods

		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.DrawString(_font, _text, _positon, _color, _rotation, _origin, _scale, _effects, LayerDepth);
		}

		#endregion
	}
}