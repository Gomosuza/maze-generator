using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering.Helpers
{
	/// <summary>
	/// Helper to store 2D texture draw calls. Later rendering in a batch.
	/// </summary>
	public class TextureEntity2D : Drawable2D
	{
		#region Fields

		private readonly Color _color;
		private readonly SpriteEffects _effects;
		private readonly Vector2 _origin;
		private readonly Vector2? _position;
		private readonly Rectangle? _rectangle;
		private readonly float _rotation;
		private readonly float _scale;
		private readonly Rectangle? _source;
		private readonly Texture2D _texture;

		#endregion

		#region Constructors

		public TextureEntity2D(Texture2D texture, Rectangle position, Color color, float layerDepth)
			: base(layerDepth)
		{
			_texture = texture;
			_rectangle = position;
			_color = color;
		}

		public TextureEntity2D(Texture2D texture, Vector2 position, Rectangle? source, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
			: base(layerDepth)
		{
			_texture = texture;
			_position = position;
			_source = source;
			_color = color;
			_rotation = rotation;
			_origin = origin * new Vector2(texture.Width, texture.Height);
			_scale = scale;
			_effects = effects;
		}

		#endregion

		#region Methods

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (_position.HasValue)
			{
				spriteBatch.Draw(_texture, _position.Value, _source, _color, _rotation, _origin, _scale, _effects, LayerDepth);
			}
			else if (_rectangle.HasValue)
			{
				spriteBatch.Draw(_texture, _rectangle.Value, _source, _color);
			}
		}

		#endregion
	}
}