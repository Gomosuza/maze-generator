using Microsoft.Xna.Framework;

namespace Engine.Rendering
{
	/// <summary>
	/// The interface for the camera, describing what portion of the level is being rendered.
	/// Responsible for transforming level to screen coordinates and vice-verca.
	/// </summary>
	public interface ICamera
	{
		#region Properties

		/// <summary>
		/// The look-at-position.
		/// Marks where the center of the screen looks at.
		/// Note that changing it will also change the position as the distance between the two is fixed for any given camera.
		/// </summary>
		Vector3 LookAt { get; set; }

		/// <summary>
		/// The position of the camera.
		/// Note that changing it will also change the look at as the distance between the two is fixed for any given camera.
		/// </summary>
		Vector3 Position { get; set; }

		Matrix Projection { get; }

		Matrix View { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Calculates the distance-to the camera for the given point.
		/// </summary>
		/// <returns></returns>
		float DistanceToCamera(Vector3 world);

		/// <summary>
		/// Calculates the distance-to the camera for the given point.
		/// </summary>
		/// <returns></returns>
		float DistanceToCameraSquared(Vector3 world);

		/// <summary>
		/// Calculates a ray that travels from the far plane to the camera's
		/// position that contains the given screen-space-point.
		/// </summary>
		/// <returns></returns>
		Ray ScreenToWorld(Vector2 screen);

		void Update(GameTime dt);

		/// <summary>
		/// Transforms a position from world to screen coordinates.
		/// </summary>
		/// <returns></returns>
		Vector2 WorldToScreen(Vector3 world);

		#endregion
	}
}