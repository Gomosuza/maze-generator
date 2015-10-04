using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Engine.Input;
using Engine.Rendering.Impl;

namespace Engine
{
	/// <summary>
	/// Abstraction over mongame provided game class.
	/// Adds vital features (keyboard, mouse, content, rendering, etc.).
	/// Use <see cref="Add"/>/<see cref="Remove"/> to add any further rendering elements.
	/// </summary>
	public abstract class GameAbstraction : Game
	{
		#region Fields

		private readonly List<IComponent> _components;

		#endregion

		#region Constructors

		protected GameAbstraction()
		{
			GraphicsDeviceManager = new GraphicsDeviceManager(this);
			_components = new List<IComponent>();
		}

		#endregion

		#region Properties

		/// <summary>
		/// The one and only graphics device manager needed by monogame.
		/// </summary>
		public GraphicsDeviceManager GraphicsDeviceManager { get; }

		/// <summary>
		/// Global keyboard manager.
		/// Will be updated automatically each update.
		/// </summary>
		public KeyboardManager KeyboardManager { get; private set; }

		/// <summary>
		/// Global mouse manager.
		/// Will be updated automatically each update.
		/// </summary>
		public MouseManager MouseManager { get; private set; }

		/// <summary>
		/// Global render context set in <see cref="Initialize"/>.
		/// If you want to use your own, just set it before your call to <see cref="Initialize"/>.
		/// </summary>
		public RenderContext RenderContext { get; protected set; }

		#endregion

		#region Methods

		public void Add(IComponent component)
		{
			_components.Add(component);
		}

		public void Remove(IComponent component)
		{
			_components.Remove(component);
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
			GraphicsDevice.Clear(Color.CornflowerBlue);

			foreach (var component in _components)
			{
				component.Render(RenderContext, gameTime);
			}
			RenderContext.Render(gameTime);
		}

		protected override void Initialize()
		{
			base.Initialize();
			Content.RootDirectory = "Content";
			// render to backbuffer by default, but only if user didn't provide a rendercontext already
			if (RenderContext == null)
			{
				RenderContext = new RenderContext(GraphicsDeviceManager, null, Content);
			}
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			MouseManager = MouseManager.GetCurrentState(MouseManager);
			KeyboardManager = KeyboardManager.GetCurrentState(KeyboardManager);

			foreach (var component in _components)
			{
				component.Update(KeyboardManager, MouseManager, gameTime);
			}
		}

		#endregion
	}
}