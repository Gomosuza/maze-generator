using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Core;
using Engine.Input;
using Engine.Rendering.Impl;

namespace Engine
{
	/// <summary>
	/// Abstraction over mongame provided game class.
	/// Adds vital features (keyboard, mouse, content, rendering, etc.).
	/// Use <see cref="Add"/>/<see cref="Remove"/> to add any further rendering elements.
	/// </summary>
	public abstract class GameAbstraction : IDisposable
	{
		#region Fields

		private readonly List<IComponent> _components;
		private readonly Game _game;

		#endregion

		#region Constructors

		protected GameAbstraction()
		{
			var helper = new XGame();
			helper.XInitialize += Initialize;
			helper.XDraw += Draw;
			helper.XUpdate += Update;
			_game = helper;

			GraphicsDeviceManager = new GraphicsDeviceManager(_game)
			{
				PreferMultiSampling = true
			};
			_components = new List<IComponent>();
		}

		#endregion

		#region Properties

		public bool IsActive => _game.IsActive;

		public GameWindow Window => _game.Window;

		public bool IsMouseVisible
		{
			get { return _game.IsMouseVisible; }
			set { _game.IsMouseVisible = value; }
		}

		public GraphicsDevice GraphicsDevice => _game.GraphicsDevice;

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
		public RenderTargetBasedRenderContext RenderContext { get; protected set; }

		public ContentManager Content => _game.Content;

		#endregion

		#region Methods

		public void Dispose()
		{
			_game.Dispose();
		}

		public void Exit()
		{
			_game.Exit();
		}

		public void Add(IComponent component)
		{
			_components.Add(component);
		}

		public IEnumerable<T> GetComponents<T>()
			where T : IComponent
		{
			return _components.OfType<T>();
		}

		public void Remove(IComponent component)
		{
			_components.Remove(component);
		}

		protected virtual void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			RenderContext.Attach();
			foreach (var component in _components)
			{
				component.Render(RenderContext, gameTime);
			}
			RenderContext.Detach();
			if (RenderContext.RenderTarget != null)
			{
				// user has actually set a rendertarget, since this is the default render context we will now draw it on screen
				var texture = RenderContext.RenderTarget;

				// force-draw to backbuffer
				RenderContext.RenderTarget = null;

				RenderContext.Attach();

				RenderContext.RenderContext2D.DrawTexture(texture, new Rectangle(0, 0, GraphicsDeviceManager.PreferredBackBufferWidth, GraphicsDeviceManager.PreferredBackBufferHeight), Color.White);
				RenderContext.Detach();

				RenderContext.RenderTarget = texture;
			}
		}

		protected virtual void Initialize()
		{
			Mouse.SetPosition(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);
			Content.RootDirectory = "Content";
			// render to backbuffer by default, but only if user didn't provide a rendercontext already
			if (RenderContext == null)
			{
				RenderContext = new RenderTargetBasedRenderContext(GraphicsDeviceManager, null, Content);
			}
		}

		protected virtual void Update(GameTime gameTime)
		{
			// reset mouse to center of screen at each frame if user set it to invisible
			bool mouseIsAlwaysCentered = !IsMouseVisible && IsActive;
			var center = new Point(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);
			MouseManager = MouseManager.GetCurrentState(MouseManager, center);

			if (mouseIsAlwaysCentered)
			{
				// reset the invisible mouse so it is always center
				Mouse.SetPosition(center.X, center.Y);
			}
			KeyboardManager = KeyboardManager.GetCurrentState(KeyboardManager);

			foreach (var component in _components)
			{
				component.Update(KeyboardManager, MouseManager, gameTime);
			}
		}

		public void Run()
		{
			_game.Run();
		}

		#endregion

		#region Nested types

		/// <summary>
		/// We don't want to derive game abstraction from Game as Game contains many properties (like "Services", "Components" that conflict with our own naming system.
		/// This would potentially lead to someone in a derived class doing "Components.OfType&lt;<see cref="IComponent"/>&gt;()" which will never yield a result as Components is of type <see cref="IGameComponent"/>.
		/// We don't use <see cref="IGameComponent"/> or other types from monogame as they are overkill for our purpose.
		/// </summary>
		private class XGame : Game
		{
			#region Methods

			protected override void Initialize()
			{
				base.Initialize();
				XInitialize?.Invoke();
			}

			protected override void Draw(GameTime gameTime)
			{
				base.Draw(gameTime);
				XDraw?.Invoke(gameTime);
			}

			protected override void Update(GameTime gameTime)
			{
				base.Update(gameTime);
				XUpdate?.Invoke(gameTime);
			}

			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);
				XInitialize = null;
				XUpdate = null;
				XDraw = null;
			}

			#endregion

			#region Other

			public event GenericEventHandler XInitialize;
			public event GenericEventHandler<GameTime> XUpdate , XDraw;

			#endregion
		}

		#endregion
	}
}