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

		Matrix Projection { get; }

		Matrix View { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Calculates the distance to the camera for the given point.
		/// </summary>
		/// <returns></returns>
		float DistanceToCamera(Vector3 world);

		/// <summary>
		/// Calculates the squared distance to the camera for the given point (faster than <see cref="DistanceToCamera"/>.
		/// </summary>
		/// <returns></returns>
		float DistanceToCameraSquared(Vector3 world);

		/// <summary>
		/// Returns the camera position.
		/// Note that this value differes between camera implementations.
		/// </summary>
		/// <returns></returns>
		Vector3 GetPosition();

		/// <summary>
		/// Calculates a ray that travels from the camera position
		/// to the far plane that contains the given screen-space-point.
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