using Microsoft.Xna.Framework;

namespace Engine.Helpers
{
	public class ColorConverter
	{
		#region Methods

		public static Vector3 Convert(Color color)
		{
			return new Vector3(color.R / 255f, color.G / 255f, color.B / 255f);
		}

		#endregion
	}
}