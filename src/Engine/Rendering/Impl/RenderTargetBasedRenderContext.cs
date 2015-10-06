using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Engine.Rendering.Meshes;

namespace Engine.Rendering.Impl
{
	/// <summary>
	/// Default render context implementation that uses a rendertarget as drawing target.
	/// Can also directly draw to back buffer.
	/// </summary>
	public class RenderTargetBasedRenderContext : IRenderContext
	{
		#region Fields

		private readonly SpriteBatchRenderContext2D _renderContext2D;
		private readonly RenderContext3D _renderContext3D;

		#endregion

		#region Constructors

		/// <summary>
		/// Render context implementation.
		/// </summary>
		/// <param name="graphicsDeviceManager"></param>
		/// <param name="renderTarget">The render target to use. Set null if it should directly render to backbuffer.</param>
		/// <param name="content"></param>
		public RenderTargetBasedRenderContext(GraphicsDeviceManager graphicsDeviceManager, RenderTarget2D renderTarget, ContentManager content)
		{
			if (graphicsDeviceManager == null)
			{
				throw new ArgumentNullException(nameof(graphicsDeviceManager));
			}
			if (graphicsDeviceManager.GraphicsDevice == null)
			{
				throw new ArgumentNullException(nameof(graphicsDeviceManager));
			}
			if (content == null)
			{
				throw new ArgumentNullException(nameof(content));
			}

			GraphicsDevice = graphicsDeviceManager.GraphicsDevice;
			GraphicsDeviceManager = graphicsDeviceManager;
			RenderTarget = renderTarget;

			Content = content;

			_renderContext2D = new SpriteBatchRenderContext2D(this);
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

		/// <summary>
		/// Get or set the render target to use.
		/// </summary>
		public RenderTarget2D RenderTarget { get; set; }

		#endregion

		#region Methods

		public void Attach()
		{
			GraphicsDevice.SetRenderTarget(RenderTarget);
			GraphicsDevice.DepthStencilState = DepthStencilState.Default;
		}

		public void Detach()
		{
			// 3D renderer draws everything when called, only 2D renderer buffers draw calls
			// so render 2D stuff in detach method as all 3D has been drawn by now
			_renderContext2D.Render();
		}

		#endregion
	}
}