using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Rendering.Meshes;

namespace Engine.Rendering.Impl
{
	/// <summary>
	/// default implementation of a 3D render context.
	/// </summary>
	public class RenderContext3D : IRenderContext3D
	{
		#region Fields

		private readonly BasicEffect _effect;
		private readonly RasterizerState _fillState;
		private readonly RasterizerState _wireFrameState;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance of the 3D renderer.
		/// </summary>
		/// <param name="attachedRenderContext"></param>
		/// <param name="cullMode">The cull mode to use for all 3D rendering.</param>
		public RenderContext3D(IRenderContext attachedRenderContext, CullMode cullMode = CullMode.CullCounterClockwiseFace)
		{
			RenderContext = attachedRenderContext;
			_fillState = new RasterizerState {CullMode = cullMode, FillMode = FillMode.Solid};
			_wireFrameState = new RasterizerState {CullMode = cullMode, FillMode = FillMode.WireFrame, DepthBias = -0.1f};

			_effect = new BasicEffect(attachedRenderContext.GraphicsDevice);
		}

		#endregion

		#region Properties

		public ICamera Camera { get; set; }

		public IRenderContext RenderContext { get; }

		#endregion

		#region Methods

		public void DrawMesh(Mesh mesh, Matrix world, Brush brush = null, Pen pen = null)
		{
			if (brush == null && pen == null)
			{
				throw new NotSupportedException("You must set either brush, pen or both. Setting none would result in nothing getting drawn");
			}

			mesh.Attach();

			SetupCamera(world);
			if (brush != null)
			{
				if (!brush.IsPrepared)
				{
					brush.Prepare(RenderContext);
				}

				RenderContext.GraphicsDevice.RasterizerState = _fillState;
				brush.Configure(_effect);

				foreach (var pass in _effect.CurrentTechnique.Passes)
				{
					pass.Apply();

					mesh.Draw();
				}
			}
			if (pen != null)
			{
				RenderContext.GraphicsDevice.RasterizerState = _wireFrameState;
				pen.Configure(_effect);

				foreach (var pass in _effect.CurrentTechnique.Passes)
				{
					pass.Apply();

					mesh.Draw();
				}
			}

			mesh.Detach();

			RenderContext.GraphicsDevice.RasterizerState = _fillState;
		}

		private void SetupCamera(Matrix world)
		{
			_effect.World = world;
			_effect.View = Camera.View;
			_effect.Projection = Camera.Projection;
		}

		#endregion
	}
}