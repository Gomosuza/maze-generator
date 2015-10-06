namespace Core
{

	#region Delegates

	public delegate void GenericEventHandler();

	public delegate void GenericEventHandler<in T>(T sender);

	public delegate void GenericEventHandler<in T, in TArg>(T sender, TArg args);

	#endregion

	public static class EventHandlerExtensions
	{
		#region Methods

		public static void SafeInvoke(this GenericEventHandler e)
		{
			e?.Invoke();
		}

		public static void SafeInvoke<T>(this GenericEventHandler<T> e, T sender)
		{
			e?.Invoke(sender);
		}

		public static void SafeInvoke<T, TArg>(this GenericEventHandler<T, TArg> e, T sender, TArg args)
		{
			e?.Invoke(sender, args);
		}

		#endregion
	}
}