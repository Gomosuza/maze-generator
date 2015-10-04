using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Input;
using Engine.Rendering.Impl;

namespace Engine
{
	public abstract class GameAbstraction : Game
	{
		#region Fields

		private readonly string _defaultFontName;

		private List<IComponent> _components;

		#endregion

		#region Constructors

		protected GameAbstraction(string defaultFontName = "DefaultFont")
		{
			_defaultFontName = defaultFontName;
			GraphicsDeviceManager = new GraphicsDeviceManager(this);
			_components = new List<IComponent>();
		}

		#endregion

		#region Properties

		public GraphicsDeviceManager GraphicsDeviceManager { get; }

		public KeyboardManager KeyboardManager { get; private set; }

		public MouseManager MouseManager { get; private set; }

		public RenderContext RenderContext { get; private set; }

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
			var font = Content.Load<SpriteFont>(_defaultFontName);
			// render to backbuffer by default
			RenderContext = new RenderContext(GraphicsDeviceManager, null, Content, font);
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