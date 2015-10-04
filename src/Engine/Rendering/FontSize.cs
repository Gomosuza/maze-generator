namespace Engine.Rendering
{

	#region Enumerations

	/// <summary>
	/// Font size enum.
	/// </summary>
	public enum FontSize
	{
		/// <summary>
		/// Smallest possible font.
		/// It is up to the implementation that uses it to decide what this looks like, however it should be smaller than <see cref="Small"/>.
		/// </summary>
		Tiny,

		/// <summary>
		/// Small font.
		/// It is up to the implementation that uses it to decide what this looks like, however it should be smaller than <see cref="Medium"/>.
		/// </summary>
		Small,

		/// <summary>
		/// Medium font.
		/// It is up to the implementation that uses it to decide what this looks like, however it should be smaller than <see cref="Large"/>.
		/// </summary>
		Medium,

		/// <summary>
		/// Large font.
		/// It is up to the implementation that uses it to decide what this looks like, however it should be smaller than <see cref="XLarge"/>.
		/// </summary>
		Large,

		/// <summary>
		/// Largest possible font.
		/// It is up to the implementation that uses it to decide what this looks like, however it should be the biggest font.
		/// </summary>
		XLarge
	}

	#endregion
}