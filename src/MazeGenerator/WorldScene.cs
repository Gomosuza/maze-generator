using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine;
using Engine.Input;
using Engine.Rendering;
using Engine.Rendering.Brushes;
using Engine.Rendering.Meshes;

namespace MazeGenerator
{
	public class WorldScene : IComponent
	{
		#region Fields

		private readonly ICamera _camera;
		private readonly Mesh _cuboid;

		#endregion

		#region Constructors

		public WorldScene(IRenderContext renderContext)
		{
			var meshBuilder = new MeshBuilder();
			meshBuilder.AddBox(new BoundingBox(Vector3.One * 10, Vector3.Zero));

			_cuboid = renderContext.MeshCreator.CreateMesh(meshBuilder);

			_camera = Camera.CreateLookAt(renderContext.GraphicsDevice, new Vector3(50, 50, 50), Vector3.Zero);
		}

		#endregion

		#region Methods

		public void Render(IRenderContext renderContext, GameTime dt)
		{
			renderContext.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
			renderContext.RenderContext3D.Camera = _camera;

			var world = Matrix.Identity;
			var brush = new SolidColorBrush(Color.Black);
			renderContext.RenderContext3D.DrawMesh(_cuboid, world, brush, new Pen(Color.White));
		}

		public void Update(KeyboardManager keyboard, MouseManager mouse, GameTime dt)
		{
			_camera.Update(dt);
		}

		#endregion
	}
}