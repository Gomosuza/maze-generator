using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine;
using Engine.Input;
using Engine.Rendering;
using Engine.Rendering.Brushes;
using Engine.Rendering.Meshes;

namespace MazeGenerator
{
	/// <summary>
	/// The world scene renders a 3D world that can be explored by the user.
	/// </summary>
	public class WorldScene : IComponent
	{
		#region Fields

		private readonly FirstPersonCamera _camera;
		private readonly Mesh _cuboid;

		#endregion

		#region Constructors

		public WorldScene(IRenderContext renderContext)
		{
			var meshBuilder = new TexturedMeshDescriptionBuilder();
			meshBuilder.AddRoom(new BoundingBox(Vector3.One * 10, Vector3.Zero), 20);

			_cuboid = renderContext.MeshCreator.CreateMesh(meshBuilder);

			_camera = new FirstPersonCamera(renderContext.GraphicsDevice, new Vector3(0, 0, 100));
		}

		#endregion

		#region Methods

		public void Render(IRenderContext renderContext, GameTime dt)
		{
			renderContext.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
			renderContext.RenderContext3D.Camera = _camera;

			var world = Matrix.Identity;
			var brush = new TexturedBrush("default");
			renderContext.RenderContext3D.DrawMesh(_cuboid, world, brush);
		}

		public void Update(KeyboardManager keyboard, MouseManager mouse, GameTime dt)
		{
			Console.WriteLine(mouse.PositionDelta);
			var time = (float)dt.ElapsedGameTime.TotalMilliseconds / 1000f;
			var delta = mouse.PositionDelta;
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
			if (keyboard.IsKeyDown(Keys.A))
			{
				stepsX -= move;
			}
			if (keyboard.IsKeyDown(Keys.D))
			{
				stepsX += move;
			}
			if (keyboard.IsKeyDown(Keys.W))
			{
				stepsZ -= move;
			}
			if (keyboard.IsKeyDown(Keys.S))
			{
				stepsZ += move;
			}
			if (keyboard.IsKeyDown(Keys.Space))
			{
				stepsY += move;
			}
			if (keyboard.IsKeyDown(Keys.LeftControl))
			{
				stepsY -= move;
			}
			_camera.Move(new Vector3(stepsX, stepsY, stepsZ) * time * 30f);

			_camera.Update(dt);
		}

		#endregion
	}
}