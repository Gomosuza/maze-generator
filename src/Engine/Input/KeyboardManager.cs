using Microsoft.Xna.Framework.Input;

namespace Engine.Input
{
	public class KeyboardManager
	{
		#region Constructors

		public KeyboardManager(KeyboardState old, KeyboardState newState)
		{
			OldState = old;
			NewState = newState;
		}

		#endregion

		#region Properties

		public KeyboardState NewState { get; }

		public KeyboardState OldState { get; }

		#endregion

		#region Methods

		public static KeyboardManager GetCurrentState(KeyboardManager oldState)
		{
			var n = Keyboard.GetState();
			var old = oldState?.NewState ?? n;
			return new KeyboardManager(old, n);
		}

		#endregion
	}
}