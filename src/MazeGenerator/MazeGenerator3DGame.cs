using System;
using System.IO;
using System.Linq;
using MazeGenerator.Entities;
using MazeGenerator.Generator;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Core;
using Core.Extensions;
using Engine;
using Engine.Diagnostics;
using Engine.Physics.Collision;
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
		private readonly Keys _down;
		private readonly Keys _forward;
		private readonly Keys _generateMaze;
		private readonly int _height;
		private readonly Keys _left;
		private readonly Keys _right;
		private readonly Keys _sprint;
		private readonly Keys _toggleCamera;
		private readonly Keys _exit;
		private readonly Keys _up;

		private readonly int _width;

		private FirstPersonCamera _camera;
		private string _details;

		#endregion

		#region Constructors

		public MazeGenerator3DGame(int width, int height)
		{
			_width = width;
			_height = height;
			Window.Title = "Maze Generator";
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
				!ReadKey(fileName, "input", "Sprint", out _sprint) ||
				!ReadKey(fileName, "options", "ToggleCamera", out _toggleCamera) ||
				!ReadKey(fileName, "options", "GenerateMaze", out _generateMaze) ||
				!ReadKey(fileName, "options", "Exit", out _exit))
			{
				throw new FileLoadException("Could not read keys from ini file. Make sure they are valid");
			}
		}

		#endregion

		#region Methods

		protected override void Draw(GameTime gameTime)
		{
			RenderContext.RenderContext3D.Camera = _camera;
			var msgBuilder = GetComponents<DebugMessageBuilder>().First();
			msgBuilder.AppendLine(_details);
			base.Draw(gameTime);
		}

		protected override void Initialize()
		{
			base.Initialize();

			FpsCounter.Enable(this);
			var message = new DebugMessageBuilder(Corner.TopRight, Color.Black);

			Add(message);

			Add(new GridAxis(RenderContext, true));

			_camera = new FirstPersonCamera(RenderContext.GraphicsDevice, Vector3.Zero, FirstPersonCamera.FirstPersonMode.Person);

			var player = new Player(_camera);

			GenerateMaze(player);
			Add(player);

			var chunkManager = GetComponents<ChunkManager>().First();
			// for first maze, place player at the starting cell

			const int playerHeight = 2;
			// position the player in the center of the starting cell
			var start = chunkManager.GetStartCell().GetBoundingBox();
			var center = (start.Min + start.Max) / 2f;
			var s = new Vector3(center.X, playerHeight, center.Z);

			_camera.SetPosition(s);

			UpdateDetails();
		}

		private void GenerateMaze(Player player)
		{
			var message = GetComponents<DebugMessageBuilder>().First();
			var entireMaze = BoundingBox.CreateMerged(new Cell(0, 0).GetBoundingBox(), new Cell(_width - 1, _height - 1).GetBoundingBox());
			var collisionEngine = new CollisionEngine(entireMaze);
			Add(collisionEngine);

			var chunkManager = new ChunkManager(RenderContext, message, collisionEngine, _width, _height);

			collisionEngine.Add(player);

			Add(chunkManager);
		}

		protected override void Update(GameTime gameTime)
		{
			if (!IsActive)
			{
				return;
			}
			base.Update(gameTime);
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
				UpdateDetails();
			}
			if (KeyboardManager.IsKeyPressed(_exit))
			{
				Exit();
			}
			if (KeyboardManager.IsKeyPressed(_generateMaze))
			{
				// clear old
				Remove(GetComponents<CollisionEngine>().First());
				Remove(GetComponents<ChunkManager>().First());

				var pl = GetComponents<Player>().First();
				GenerateMaze(pl);

				// player must be updated after the other components, therefore remove it and add it again
				Remove(pl);
				Add(pl);
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
			var move = KeyboardManager.IsKeyDown(_sprint) ? 1.5f : 0.5f;
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
			switch (_camera.Mode)
			{
				case FirstPersonCamera.FirstPersonMode.Plane:
					_camera.Mode = FirstPersonCamera.FirstPersonMode.Person;
					break;
				case FirstPersonCamera.FirstPersonMode.Person:
					_camera.Mode = FirstPersonCamera.FirstPersonMode.Plane;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void UpdateDetails()
		{
			_details = $"Current camera mode ({_toggleCamera}): {_camera.Mode} " + Environment.NewLine +
					   $"Press {_generateMaze} to generate a new maze" + Environment.NewLine +
					   $"Press {_exit} to exit";
		}

		#endregion
	}
}