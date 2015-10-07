using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Helpers;

namespace Engine.Rendering.Brushes
{
	/// <summary>
	/// A brush that paints with a single color.
	/// </summary>
	public sealed class SolidColorBrush : Brush
	{
		#region Fields

		private Color _color;
		private Vector3 _precalculated;
		private SamplerState _sampler;

		#endregion

		#region Constructors

		public SolidColorBrush(Color color)
		{
			Color = color;
		}

		#endregion

		#region Properties

		public Color Color
		{
			get { return _color; }
			set
			{
				_color = value;
				_precalculated = ColorConverter.Convert(Color);
			}
		}

		internal override bool IsPrepared
		{
			get { return _sampler != null; }
		}

		#endregion

		#region Methods

		public override string ToString()
		{
			return Color.ToString();
		}

		internal override void Configure(BasicEffect effect)
		{
			effect.LightingEnabled = false;
			effect.AmbientLightColor = Vector3.Zero;
			effect.DiffuseColor = _precalculated;
			effect.FogEnabled = false;
			effect.VertexColorEnabled = false;
			effect.TextureEnabled = false;

			effect.GraphicsDevice.SamplerStates[0] = _sampler;
		}

		internal override void Prepare(IRenderContext renderContext)
		{
			_sampler = new SamplerState
			{
				Filter = TextureFilter.LinearMipPoint
			};
		}

		#endregion
	}
}