using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Engine.Rendering.Meshes;

namespace Engine.Rendering.Impl
{
	public class RenderContext : IRenderContext
	{
		#region Fields

		private readonly SpriteBatchRenderContext2D _renderContext2D;
		private readonly RenderContext3D _renderContext3D;

		#endregion

		#region Constructors

		public RenderContext(GraphicsDeviceManager graphicsDeviceManager, RenderTarget2D renderTarget, ContentManager content, SpriteFont defaultFont)
		{
			if (graphicsDeviceManager == null)
			{
				throw new ArgumentNullException(nameof(graphicsDeviceManager));
			}
			if (graphicsDeviceManager.GraphicsDevice == null)
			{
				throw new ArgumentNullException(nameof(graphicsDeviceManager));
			}

			GraphicsDevice = graphicsDeviceManager.GraphicsDevice;
			GraphicsDeviceManager = graphicsDeviceManager;
			RenderTarget = renderTarget;

			Content = content;

			_renderContext2D = new SpriteBatchRenderContext2D(this, defaultFont);
			_renderContext3D = new RenderContext3D(this);

			MeshCreator = new MeshCreator(GraphicsDevice);
		}

		#endregion

		#region Properties

		public ContentManager Content { get; }

		public GraphicsDevice GraphicsDevice { get; }

		public GraphicsDeviceManager GraphicsDeviceManager { get; }

		public IMeshCreator MeshCreator { get; }

		public IRenderContext2D RenderContext2D
		{
			get { return _renderContext2D; }
		}

		public IRenderContext3D RenderContext3D
		{
			get { return _renderContext3D; }
		}

		public RenderTarget2D RenderTarget { get; set; }

		#endregion

		#region Methods

		public void Render(GameTime dt)
		{
			GraphicsDevice.SetRenderTarget(RenderTarget);
			_renderContext2D.Render(dt);
			_renderContext3D.Render(dt);
		}

		#endregion
	}
}