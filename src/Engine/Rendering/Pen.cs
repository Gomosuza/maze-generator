using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Rendering.Brushes;

namespace Engine.Rendering
{
	/// <summary>
	/// A pen that outlines a drawing.
	/// This will generally result in the wireframe being drawn over whichever object it is attached to.
	/// </summary>
	public sealed class Pen
	{
		#region Fields

		private readonly Brush _brush;

		#endregion

		#region Constructors

		public Pen(Color color)
		{
			_brush = new SolidColorBrush(color);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Configures the given effect to use this brush.
		/// </summary>
		/// <param name="effect"></param>
		internal void Configure(BasicEffect effect)
		{
			_brush.Configure(effect);
		}

		#endregion
	}
}