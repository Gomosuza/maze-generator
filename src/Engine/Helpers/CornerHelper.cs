using System;
using Microsoft.Xna.Framework;

namespace Engine.Helpers
{
	public static class CornerHelper
	{
		#region Methods

		/// <summary>
		/// Helper that will return position and origin for drawing 2D elements in the specific corner.
		/// Simple use the provided <paramref name="position"/> and <paramref name="origin"/> in your draw call.
		/// </summary>
		/// <param name="corner">The corner for which to get the values.</param>
		/// <param name="screenSize">The size of the screen, required for correct calculations of all but <see cref="Corner.TopLeft"/>.</param>
		/// <param name="position"></param>
		/// <param name="origin"></param>
		public static void GetPositionAndOrigin(Corner corner, Vector2 screenSize, out Vector2 position, out Vector2 origin)
		{
			switch (corner)
			{
				case Corner.TopLeft:
					position = Vector2.Zero;
					origin = Vector2.Zero;
					break;
				case Corner.TopRight:
					position = new Vector2(screenSize.X, 0);
					origin = new Vector2(1, 0);
					break;
				case Corner.BottomRight:
					position = screenSize;
					origin = Vector2.One;
					break;
				case Corner.BottomLeft:
					position = new Vector2(0, screenSize.Y);
					origin = new Vector2(0, 1);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		#endregion
	}
}