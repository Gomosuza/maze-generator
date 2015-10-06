using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

		private readonly Mesh _cuboid;

		#endregion

		#region Constructors

		public WorldScene(IRenderContext renderContext)
		{
			var meshBuilder = new TexturedMeshDescriptionBuilder();
			meshBuilder.AddBox(new BoundingBox(Vector3.One * 5, Vector3.Zero), 10);

			_cuboid = renderContext.MeshCreator.CreateMesh(meshBuilder);
		}

		#endregion

		#region Methods

		public void Render(IRenderContext renderContext, GameTime dt)
		{
			renderContext.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

			var world = Matrix.Identity;
			var brush = new TexturedBrush("default");
			renderContext.RenderContext3D.DrawMesh(_cuboid, world, brush);
		}

		public void Update(KeyboardManager keyboard, MouseManager mouse, GameTime dt)
		{
		}

		#endregion
	}
}