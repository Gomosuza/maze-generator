using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Rendering.Helpers;

namespace Engine.Rendering.Impl
{
	/// <summary>
	/// 2D render context that uses the spritebatch to make its draw calls.
	/// </summary>
	public class SpriteBatchRenderContext2D : IRenderContext2D
	{
		#region Fields

		private readonly List<Drawable2D> _entities;
		private readonly SpriteBatch _spriteBatch;

		#endregion

		#region Constructors

		public SpriteBatchRenderContext2D(IRenderContext attachedRenderContext)
		{
			RenderContext = attachedRenderContext;
			_spriteBatch = new SpriteBatch(RenderContext.GraphicsDevice);
			_entities = new List<Drawable2D>();
		}

		#endregion

		#region Properties

		public IRenderContext RenderContext { get; }

		#endregion

		#region Methods

		public void DrawString(string text, Vector2 position, Color color, FontSize size = FontSize.Medium, float layerDepth = 0f)
		{
			DrawString(text, position, Vector2.Zero, color, size, layerDepth);
		}

		public void DrawString(string text, Vector2 position, Vector2 origin, Color color, FontSize size = FontSize.Medium, float layerDepth = 0)
		{
			float scale;
			var font = GetFontAndScale(size, out scale);

			// no need for scale here, spriteBatch seems to apply it after doing origin calculations
			var fontScale = font.MeasureString(text);
			_entities.Add(new FontEntity2D(font, text, position, color, 0f, origin * fontScale, Vector2.One * scale, SpriteEffects.None, layerDepth));
		}

		public void DrawTexture(Texture2D texture, Vector2 position, Vector2 origin, float scale = 1f, float layerDepth = 0)
		{
			DrawTexture(texture, position, origin, scale, Color.White, layerDepth);
		}

		public void DrawTexture(Texture2D texture, Vector2 position, Vector2 origin, float scale, Color color, float layerDepth = 0)
		{
			_entities.Add(new TextureEntity2D(texture, position, null, color, 0f, origin * new Vector2(texture.Width, texture.Height), scale, SpriteEffects.None, layerDepth));
		}

		public Vector2 MeasureString(string message, FontSize fontSize)
		{
			float scale;
			var font = GetFontAndScale(fontSize, out scale);
			return font.MeasureString(message) * scale;
		}

		public void Render()
		{
			_spriteBatch.Begin();

			// TODO: might be hideous slow for many entities, do we really need to sort here?

			// need to perform a stable sort, otherwise - order might change for entities with same value between drae calls (which would cause flickering)
			var sorted = _entities.OrderBy(x => x, Comparer<Drawable2D>.Create((a, b) => a.LayerDepth.CompareTo(b.LayerDepth))).ToList();

			_entities.Clear();
			foreach (var e in sorted)
			{
				e.Draw(_spriteBatch);
			}
			_spriteBatch.End();
		}

		private SpriteFont GetFontAndScale(FontSize size, out float scale)
		{
			switch (size)
			{
				case FontSize.Tiny:
					scale = 0.5f; // 8
					return StaticResourceCache.GetSmallFont(RenderContext.Content);
				case FontSize.Small:
					scale = 1f; // 16
					return StaticResourceCache.GetSmallFont(RenderContext.Content);
				case FontSize.Medium:
					scale = 0.5f; // 32
					return StaticResourceCache.GetLargeFont(RenderContext.Content);
				case FontSize.Large:
					scale = 0.75f; // 48
					return StaticResourceCache.GetLargeFont(RenderContext.Content);
				case FontSize.XLarge:
					scale = 1f; // 64
					return StaticResourceCache.GetLargeFont(RenderContext.Content);
				default:
					throw new ArgumentOutOfRangeException(nameof(size), size, null);
			}
		}

		#endregion
	}
}