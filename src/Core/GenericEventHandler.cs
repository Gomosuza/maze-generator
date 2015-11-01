namespace Core
{

	#region Delegates

	public delegate void GenericEventHandler();

	public delegate void GenericEventHandler<in T>(T sender);

	public delegate void GenericEventHandler<in T, in TArg>(T sender, TArg args);

	#endregion
}