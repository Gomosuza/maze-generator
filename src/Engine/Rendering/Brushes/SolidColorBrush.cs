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
			get { return true; }
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
		}

		internal override void Prepare(IRenderContext renderContext)
		{
		}

		#endregion
	}
}