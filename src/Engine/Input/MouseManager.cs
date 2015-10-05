using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine.Input
{
	/// <summary>
	/// The mouse manager handles all mouse events.
	/// </summary>
	public class MouseManager
	{
		#region Constructors

		private MouseManager(MouseState old, MouseState newState, Point oldMousePosition)
		{
			OldState = old;
			NewState = newState;
			PositionDelta = (NewState.Position - oldMousePosition).ToVector2();
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

		/// <summary>
		/// Gets the delta the mouse has moved from the previous state.
		/// </summary>
		public Vector2 PositionDelta { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Call to get a new manager for the current state.
		/// </summary>
		/// <param name="oldState"></param>
		/// <param name="currentPosition">If you used <see cref="Mouse.SetPosition"/> in a previous call you must set the new mouse position manually, otherwise the <see cref="PositionDelta"/> calculation will be wrong.</param>
		/// <returns></returns>
		public static MouseManager GetCurrentState(MouseManager oldState, Point? currentPosition)
		{
			var n = Mouse.GetState();
			var old = oldState?.NewState ?? n;
			return new MouseManager(old, n, currentPosition ?? old.Position);
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