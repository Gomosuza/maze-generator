using Microsoft.Xna.Framework.Input;

namespace Engine.Input
{
	/// <summary>
	/// The mouse manager handles all mouse events.
	/// </summary>
	public class MouseManager
	{
		#region Constructors

		private MouseManager(MouseState old, MouseState newState)
		{
			OldState = old;
			NewState = newState;
		}

		#endregion

		#region Properties

		/// <summary>
		/// The state from the current update.
		/// </summary>
		public MouseState NewState { get; }

		/// <summary>
		/// The state from the previous update.
		/// </summary>
		public MouseState OldState { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Call to get a new manager for the current state.
		/// </summary>
		/// <param name="oldState"></param>
		/// <returns></returns>
		public static MouseManager GetCurrentState(MouseManager oldState)
		{
			var n = Mouse.GetState();
			var old = oldState?.NewState ?? n;
			return new MouseManager(old, n);
		}

		/// <summary>
		/// Returns if the left button has just been clicked (was pressed and has just been released).
		/// </summary>
		/// <returns></returns>
		public bool IsLeftButtonClicked()
		{
			return OldState.LeftButton == ButtonState.Pressed &&
			       NewState.LeftButton == ButtonState.Released;
		}

		#endregion
	}
}