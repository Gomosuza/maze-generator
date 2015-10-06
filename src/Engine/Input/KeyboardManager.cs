using Microsoft.Xna.Framework.Input;

namespace Engine.Input
{
	/// <summary>
	/// The keyboard manager handles all keyboard events.
	/// </summary>
	public class KeyboardManager
	{
		#region Constructors

		private KeyboardManager(KeyboardState old, KeyboardState newState)
		{
			OldState = old;
			NewState = newState;
		}

		#endregion

		#region Properties

		/// <summary>
		/// The state from the current update
		/// </summary>
		public KeyboardState NewState { get; }

		/// <summary>
		/// The state from the previous update.
		/// </summary>
		public KeyboardState OldState { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Call to get the new state.
		/// </summary>
		/// <param name="oldState"></param>
		/// <returns></returns>
		public static KeyboardManager GetCurrentState(KeyboardManager oldState)
		{
			var n = Keyboard.GetState();
			var old = oldState?.NewState ?? n;
			return new KeyboardManager(old, n);
		}

		/// <summary>
		/// Returns true if the specific key is currently pressed.
		/// </summary>
		/// <returns></returns>
		public bool IsKeyDown(Keys key)
		{
			return NewState.IsKeyDown(key);
		}

		/// <summary>
		/// Returns whether a key has been pressed.
		/// A key press is defined as the key being pressed in the previous update and being released just now.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool IsKeyPressed(Keys key)
		{
			return OldState.IsKeyDown(key) && NewState.IsKeyUp(key);
		}

		#endregion
	}
}