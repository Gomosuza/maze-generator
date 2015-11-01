using Microsoft.Xna.Framework;
using Engine.Physics.Collision;
using Engine.Rendering;

namespace MazeGenerator
{
	public class Player : ICollidable
	{
		#region Fields

		private readonly FirstPersonCamera _camera;

		#endregion

		#region Constructors

		public Player(FirstPersonCamera camera)
		{
			_camera = camera;
		}

		#endregion

		#region Properties

		public BoundingBox BoundingBox => new BoundingBox(_camera.GetPosition() - new Vector3(1, 0, 1), _camera.GetPosition() + new Vector3(1, 0, 1));

		public bool IsStatic => false;

		#endregion

		#region Methods

		public bool Collides(ICollidable other)
		{
			// only collide in first person mode
			return _camera.Mode == FirstPersonCamera.FirstPersonMode.Person;
		}

		#endregion
	}
}