using Microsoft.Xna.Framework.Input;

namespace Engine.Input
{
	public class MouseManager
	{
		#region Constructors

		public MouseManager(MouseState old, MouseState newState)
		{
			OldState = old;
			NewState = newState;
		}

		#endregion

		#region Properties

		public MouseState NewState { get; }

		public MouseState OldState { get; }

		#endregion

		#region Methods

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