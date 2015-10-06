using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering.Brushes
{
	/// <summary>
	/// A brush suitable for <see cref="VertexPositionColor"/> or any other vertex with color.
	/// Will always use the color provided by the vertices.
	/// </summary>
	public class VertexColorBrush : Brush
	{
		#region Fields

		private static VertexColorBrush _vertexColorBrush;

		#endregion

		#region Constructors

		public VertexColorBrush()
		{
			IsPrepared = true;
		}

		#endregion

		#region Properties

		public static VertexColorBrush Default
		{
			get { return _vertexColorBrush ?? (_vertexColorBrush = new VertexColorBrush()); }
		}

		internal override bool IsPrepared { get; }

		#endregion

		#region Methods

		internal override void Configure(BasicEffect effect)
		{
			effect.LightingEnabled = false;
			effect.FogEnabled = false;
			effect.VertexColorEnabled = true;
			effect.TextureEnabled = false;
		}

		internal override void Prepare(IRenderContext renderContext)
		{
		}

		#endregion
	}
}