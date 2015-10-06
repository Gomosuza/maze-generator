using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Core;
using Core.Extensions;
using Engine;
using Engine.Diagnostics;
using Engine.Rendering;
using Engine.Rendering.Components;

namespace MazeGenerator
{
	/// <summary>
	/// Actual game implementation.
	/// Handles loading, configuration, etc.
	/// </summary>
	public class MazeGenerator3DGame : GameAbstraction
	{
		#region Fields

		private readonly Keys _forward, _left, _backward, _right;

		private FirstPersonCamera _camera;

		#endregion

		#region Constructors

		public MazeGenerator3DGame()
		{
			var fileName = "settings.ini";
			var exe = GetType().Assembly;
			fileName = Path.Combine(exe.GetDirectory(), fileName);

			int w;
			if (!int.TryParse(IniHelper.ReadValue(fileName, "preferences", "width"), out w))
			{
				w = 1280;
			}
			int h;
			if (!int.TryParse(IniHelper.ReadValue(fileName, "preferences", "height"), out h))
			{
				h = 800;
			}

			var fullscreen = "true".Equals(IniHelper.ReadValue(fileName, "preferences", "fullscreen"), StringComparison.InvariantCultureIgnoreCase);

			GraphicsDeviceManager.IsFullScreen = fullscreen;
			GraphicsDeviceManager.PreferredBackBufferWidth = w;
			GraphicsDeviceManager.PreferredBackBufferHeight = h;
			GraphicsDeviceManager.ApplyChanges();

			IsMouseVisible = false;

			if (!ReadKey(fileName, "input", "Forward", out _forward) ||
			    !ReadKey(fileName, "input", "Left", out _left) ||
			    !ReadKey(fileName, "input", "Backward", out _backward) ||
			    !ReadKey(fileName, "input", "Right", out _right))
			{
				throw new FileLoadException("Could not read keys from ini file. Make sure they are valid");
			}
		}

		#endregion

		#region Methods

		private static bool ReadKey(string file, string section, string key, out Keys k)
		{
			var c = IniHelper.ReadValue(file, section, key);
			if (!string.IsNullOrEmpty(c))
			{
				return Enum.TryParse(c, true, out k);
			}
			k = Keys.None;
			return false;
		}

		protected override void Initialize()
		{
			base.Initialize();
			_camera = new FirstPersonCamera(RenderContext.GraphicsDevice, new Vector3(0, 0, 100), FirstPersonCamera.FirstPersonMode.Person);

			FpsCounter.Enable(this);
			Add(new WorldScene(RenderContext));
			Add(new GridAxis(RenderContext, true));
		}

		protected override void Update(GameTime gameTime)
		{
			if (!IsActive)
			{
				return;
			}
			base.Update(gameTime);
#if DEBUG
			// quickly exit while debugging
			if (KeyboardManager.IsKeyDown(Keys.Escape))
			{
				Exit();
			}
#endif
			HandleInput(gameTime);
		}

		private void HandleInput(GameTime dt)
		{
			var time = (float)dt.ElapsedGameTime.TotalMilliseconds / 1000f;
			var delta = MouseManager.PositionDelta;
			float rotationY = 0, rotationX = 0;
			const float rotateDelta = 200f;
			if (delta.X != 0)
			{
				rotationY = delta.X * rotateDelta * time;
			}
			if (delta.Y != 0)
			{
				rotationX = delta.Y * rotateDelta * time;
			}
			_camera.AddHorizontalRotation(rotationY * time / 50f);
			_camera.AddVerticalRotation(rotationX * time / 50f);

			int stepsX = 0, stepsZ = 0, stepsY = 0;
			const int move = 1;
			if (KeyboardManager.IsKeyDown(_left))
			{
				stepsX -= move;
			}
			if (KeyboardManager.IsKeyDown(_right))
			{
				stepsX += move;
			}
			if (KeyboardManager.IsKeyDown(_forward))
			{
				stepsZ -= move;
			}
			if (KeyboardManager.IsKeyDown(_backward))
			{
				stepsZ += move;
			}
			_camera.Move(new Vector3(stepsX, stepsY, stepsZ) * time * 30f);

			_camera.Update(dt);
		}

		protected override void Draw(GameTime gameTime)
		{
			RenderContext.RenderContext3D.Camera = _camera;
			base.Draw(gameTime);
		}

		#endregion
	}
}