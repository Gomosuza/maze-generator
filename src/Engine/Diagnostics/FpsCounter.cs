using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Engine.Input;
using Engine.Rendering;

namespace Engine.Diagnostics
{
	/// <summary>
	/// Helper component. Use static methods <see cref="Enable"/>/<see cref="Disable"/> to use it.
	/// </summary>
	public class FpsCounter : IComponent
	{
		#region Fields

		/// <summary>
		/// Samples over 2 seconds if we run with 60 fps.
		/// </summary>
		private const int MaximumSamples = 120;

		private static FpsCounter _instance;

		private readonly Corner _corner;
		private readonly Color _fontColor;
		private readonly float[] _sampleBuffer;

		private float _currentFramesPerSecond;
		private int _sampleIndex;

		#endregion

		#region Constructors

		private FpsCounter(Corner corner, Color color)
		{
			_corner = corner;
			_fontColor = color;
			_sampleBuffer = new float[MaximumSamples];
		}

		#endregion

		#region Properties

		/// <summary>
		/// The average frames rendered over the last few seconds.
		/// </summary>
		private float AverageFramesPerSecond { get; set; }

		#endregion

		#region Methods

		public void Render(IRenderContext renderContext, GameTime dt)
		{
			Vector2 position;
			Vector2 origin;
			var screenSize = new Vector2(renderContext.GraphicsDeviceManager.PreferredBackBufferWidth, renderContext.GraphicsDeviceManager.PreferredBackBufferHeight);
			switch (_corner)
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
			renderContext.RenderContext2D.DrawString($"FPS: {Math.Round(AverageFramesPerSecond):000}", position, origin, _fontColor, FontSize.Tiny);
		}

		public void Update(KeyboardManager keyboard, MouseManager mouse, GameTime dt)
		{
			_currentFramesPerSecond = (float)(1.0f / dt.ElapsedGameTime.TotalSeconds);

			_sampleBuffer[_sampleIndex] = _currentFramesPerSecond;
			_sampleIndex = ++_sampleIndex % _sampleBuffer.Length;

			// initially we are filled with zeros so ignore those values
			AverageFramesPerSecond = _sampleBuffer.Where(i => i > 0).Average(i => i);
		}

		/// <summary>
		/// Disables the FPS counter on the specific entity manager.
		/// </summary>
		/// <param name="manager"></param>
		public static void Disable(GameAbstraction manager)
		{
			manager.Remove(_instance);
			_instance = null;
		}

		/// <summary>
		/// Enables the FPS counter on the specific entity manager.
		/// Optionally set the color to use and the location to use.
		/// </summary>
		/// <param name="manager"></param>
		/// <param name="corner">The corner to draw the FPS counter to.</param>
		/// <param name="customFontColor">The color to use for drawing. Defaults to black.</param>
		public static void Enable(GameAbstraction manager, Corner corner = Corner.TopLeft, Color? customFontColor = null)
		{
			if (_instance != null)
			{
				return;
			}

			var color = customFontColor ?? Color.Black;
			manager.Add(_instance = new FpsCounter(corner, color));
		}

		#endregion
	}
}