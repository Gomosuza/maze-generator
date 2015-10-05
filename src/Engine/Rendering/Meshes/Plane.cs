namespace Engine.Rendering.Meshes
{

	#region Enumerations

	/// <summary>
	/// The plane enum is used to describe an axis-aligned faces direction.
	/// XNA uses a right hand coordinate system (positive Y is up, positive X is right and positive Z is towards the user facing the monitor).
	/// </summary>
	public enum Plane
	{
		/// <summary>
		/// The face is facing the negative X coordinate.
		/// Aka "left" in the coordinate system.
		/// </summary>
		NegativeX,

		/// <summary>
		/// The face is facing the positive X coordinate.
		/// Aka "right" in the coordinate system.
		/// </summary>
		PositiveX,

		/// <summary>
		/// The face is facing the negative Y coordinate.
		/// Aka "down" in the coordinate system.
		/// </summary>
		NegativeY,

		/// <summary>
		/// The face is facing the positive Y coordinate.
		/// Aka "up" in the coordinate system.
		/// </summary>
		PositiveY,

		/// <summary>
		/// The face is facing the negative Z coordinate.
		/// Aka "in" in the coordinate system.
		/// </summary>
		NegativeZ,

		/// <summary>
		/// The face is facing the positive Z coordinate.
		/// Aka "out" in the coordinate system.
		/// </summary>
		PositiveZ,
	}

	#endregion
}