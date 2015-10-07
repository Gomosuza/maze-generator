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

		private readonly Keys _backward;
		private readonly Keys _boost;
		private readonly Keys _down;
		private readonly Keys _forward;
		private readonly Keys _left;
		private readonly Keys _right;
		private readonly Keys _toggleCamera;
		private readonly Keys _up;

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
			    !ReadKey(fileName, "input", "Right", out _right) ||
			    !ReadKey(fileName, "input", "Up", out _up) ||
			    !ReadKey(fileName, "input", "Down", out _down) ||
			    !ReadKey(fileName, "input", "ToggleCamera", out _toggleCamera) ||
			    !ReadKey(fileName, "input", "Boost", out _boost))
			{
				throw new FileLoadException("Could not read keys from ini file. Make sure they are valid");
			}
		}

		#endregion

		#region Methods

		protected override void Draw(GameTime gameTime)
		{
			RenderContext.RenderContext3D.Camera = _camera;
			base.Draw(gameTime);
		}

		protected override void Initialize()
		{
			base.Initialize();

			FpsCounter.Enable(this);
			var message = new DebugMessageBuilder(Corner.TopRight, Color.Black);

			Add(message);
			var world = new WorldScene(RenderContext, message);

			var box = world.GetEmptyCellCloseToCenter();
			const int playerHeight = 2;
			var center = new Vector3(box.Max.X + box.Min.X, 0, box.Max.Z + box.Min.Z) / 2;
			_camera = new FirstPersonCamera(RenderContext.GraphicsDevice, new Vector3(center.X, playerHeight, center.Z), FirstPersonCamera.FirstPersonMode.Person);

			Add(world);
			Add(new GridAxis(RenderContext, true));

			UpdateWindowTitle();
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

		private void HandleInput(GameTime dt)
		{
			if (KeyboardManager.IsKeyPressed(_toggleCamera))
			{
				ToggleCameraMode();
				UpdateWindowTitle();
			}
			var time = (float)dt.ElapsedGameTime.TotalMilliseconds / 1000f;
			var delta = MouseManager.PositionDelta;

			float rotationY = 0, rotationX = 0;
			const float rotateDelta = 150f;
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

			float stepsX = 0, stepsZ = 0, stepsY = 0;
			var move = KeyboardManager.IsKeyDown(_boost) ? 1.5f : 0.5f;
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
			if (_camera.Mode == FirstPersonCamera.FirstPersonMode.Plane)
			{
				if (KeyboardManager.IsKeyDown(_up))
				{
					stepsY += move;
				}
				if (KeyboardManager.IsKeyDown(_down))
				{
					stepsY -= move;
				}
			}
			_camera.Move(new Vector3(stepsX, stepsY, stepsZ) * time * 30f);

			_camera.Update(dt);
		}

		private void ToggleCameraMode()
		{
			var h = _camera.UpDownRotation;
			var v = _camera.LeftRightRotation;
			switch (_camera.Mode)
			{
				case FirstPersonCamera.FirstPersonMode.Plane:
					_camera = new FirstPersonCamera(GraphicsDevice, _camera.Position, FirstPersonCamera.FirstPersonMode.Person, _camera.FarZ);
					break;
				case FirstPersonCamera.FirstPersonMode.Person:
					_camera = new FirstPersonCamera(GraphicsDevice, _camera.Position, FirstPersonCamera.FirstPersonMode.Plane, _camera.FarZ);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			_camera.SetRotation(h, v);
		}

		private void UpdateWindowTitle()
		{
			Window.Title = $"Maze Generator - {_toggleCamera} to toggle camera. Current mode: {_camera.Mode}";
		}

		#endregion
	}
}