using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Engine.Input;
using Engine.Rendering;

namespace Engine.Diagnostics
{
	public class FpsCounter : IComponent
	{
		#region Fields

		/// <summary>
		/// Samples over 2 seconds if we run with 60 fps.
		/// </summary>
		private const int MaximumSamples = 120;

		private static FpsCounter _instance;

		private readonly Color _fontColor;
		private readonly float[] _sampleBuffer;

		private float _currentFramesPerSecond;
		private int _sampleIndex;

		#endregion

		#region Constructors

		private FpsCounter(Color? customColor)
		{
			_fontColor = customColor ?? Color.Black;
			_sampleBuffer = new float[MaximumSamples];
		}

		#endregion

		#region Properties

		private float AverageFramesPerSecond { get; set; }

		#endregion

		#region Methods

		public void Render(IRenderContext renderContext, GameTime dt)
		{
			renderContext.RenderContext2D.DrawString($"FPS: {Math.Round(AverageFramesPerSecond):000}", Vector2.Zero, _fontColor, FontSize.Tiny);
		}

		public void Update(KeyboardManager keyboard, MouseManager mouse, GameTime dt)
		{
			_currentFramesPerSecond = (float)(1.0f / dt.ElapsedGameTime.TotalSeconds);

			_sampleBuffer[_sampleIndex] = _currentFramesPerSecond;
			_sampleIndex = ++_sampleIndex % _sampleBuffer.Length;

			// initially we are filled with zeros so ignore those values
			AverageFramesPerSecond = _sampleBuffer.Where(i => i != 0).Average(i => i);
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
		/// </summary>
		/// <param name="manager"></param>
		/// <param name="customFontColor"></param>
		public static void Enable(GameAbstraction manager, Color? customFontColor = null)
		{
			if (_instance != null)
			{
				return;
			}

			manager.Add(_instance = new FpsCounter(customFontColor));
		}

		#endregion
	}
}