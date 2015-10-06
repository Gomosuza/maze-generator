using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering.Brushes
{
	/// <summary>
	/// A brush that paints with a texture.
	/// </summary>
	public class TexturedBrush : Brush
	{
		#region Fields

		private SamplerState _sampler;
		private Texture2D _texture;

		#endregion

		#region Constructors

		public TexturedBrush(string texture)
		{
			if (string.IsNullOrWhiteSpace(texture))
			{
				throw new ArgumentException();
			}

			Texture = texture;
		}

		public TexturedBrush(Texture2D texture)
		{
			if (texture == null)
			{
				throw new ArgumentNullException(nameof(texture));
			}

			_texture = texture;
		}

		protected TexturedBrush()
		{
		}

		#endregion

		#region Properties

		public virtual int Height
		{
			get { return _texture?.Height ?? 0; }
		}

		public virtual Vector2 Size
		{
			get { return new Vector2(Width, Height); }
		}

		public string Texture { get; }

		public virtual int Width
		{
			get { return _texture?.Width ?? 0; }
		}

		internal override bool IsPrepared
		{
			get { return _texture != null; }
		}

		#endregion

		#region Methods

		public override string ToString()
		{
			return Texture;
		}

		internal override void Configure(BasicEffect effect)
		{
			effect.AmbientLightColor = Vector3.Zero;
			effect.DiffuseColor = Vector3.One;
			effect.LightingEnabled = false;
			effect.Texture = _texture;
			effect.TextureEnabled = true;
			effect.FogEnabled = false;
			effect.VertexColorEnabled = false;

			// use better filter so textures don't flicker when the screen is moved
			_texture.GraphicsDevice.SamplerStates[0] = _sampler;
		}

		internal override void Prepare(IRenderContext renderContext)
		{
			_texture = renderContext.Content.Load<Texture2D>(Texture);
			_sampler = new SamplerState
			{
				Filter = TextureFilter.LinearMipPoint
			};
		}

		#endregion
	}
}