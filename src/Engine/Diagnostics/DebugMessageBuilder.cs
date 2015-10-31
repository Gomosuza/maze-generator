using System;
using System.Text;
using Microsoft.Xna.Framework;
using Engine.Helpers;
using Engine.Input;
using Engine.Rendering;

namespace Engine.Diagnostics
{
	/// <summary>
	/// Helper that allows to display text.
	/// The content is removed each frame and must be added again.
	/// </summary>
	public class DebugMessageBuilder : IComponent
	{
		#region Fields

		private const FontSize FontSize = Rendering.FontSize.Small;

		private readonly Color _color;
		private readonly Corner _corner;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new message builder that displays all messages at once.
		/// </summary>
		/// <param name="corner">The corner where to display the messages.</param>
		/// <param name="color">THe font color.</param>
		public DebugMessageBuilder(Corner corner, Color color)
		{
			_color = color;
			_corner = corner;
			StringBuilder = new StringBuilder();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Add your content to the builder.
		/// Recommended to always use AppendLine, so the next added content isn't added to the previous line.
		/// </summary>
		public StringBuilder StringBuilder { get; }

		#endregion

		#region Methods

		public void Render(IRenderContext renderContext, GameTime dt)
		{
			var message = StringBuilder.ToString();
			Vector2 position;
			Vector2 origin;
			var screenSize = new Vector2(renderContext.GraphicsDeviceManager.PreferredBackBufferWidth, renderContext.GraphicsDeviceManager.PreferredBackBufferHeight);
			CornerHelper.GetPositionAndOrigin(_corner, screenSize, out position, out origin);
			if (_corner == Corner.BottomLeft || _corner == Corner.TopLeft)
			{
				renderContext.RenderContext2D.DrawString(message, position, origin, _color, FontSize);
			}
			else
			{
				// XNA doesn't support true right-align (will take widest line which results in most lines drawing offset from the right side).
				// rendering each line by itself is a simple fix, not really "cheap" but there won't ever be enough text to cause rendering/performance issues.
				var lines = message.Split(new[]
				{
					'\n', '\r'
				}, StringSplitOptions.RemoveEmptyEntries);
				foreach (var line in lines)
				{
					renderContext.RenderContext2D.DrawString(line, position, origin, _color, FontSize);
					var h = renderContext.RenderContext2D.MeasureString(line, FontSize).Y;
					// need to move up if we are in the bottom corner, otherwise move down
					position.Y += _corner == Corner.BottomRight ? -h : h;
				}
			}
		}

		public void Update(KeyboardManager keyboard, MouseManager mouse, GameTime dt)
		{
			StringBuilder.Clear();
		}

		#endregion
	}
}