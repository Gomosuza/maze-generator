namespace Engine.Input
{

	#region Enumerations

	public enum InputState
	{
		/// <summary>
		/// The input processing may continue.
		/// </summary>
		None,

		/// <summary>
		/// Indicates that the specific event was now handled by the component and further processing will not happen.
		/// </summary>
		Handled
	}

	#endregion
}