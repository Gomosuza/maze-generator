using System;
using Microsoft.Xna.Framework;
using Engine;
using Engine.Input;
using Engine.Physics.Collision;
using Engine.Rendering;

namespace MazeGenerator.Entities
{
	public class Player : ICollidable, IComponent
	{
		#region Fields

		private readonly FirstPersonCamera _camera;

		private Vector3 _last;

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

		public void CollisionResponse(ICollidable other)
		{
			if (other is Wall)
			{
				var w = (Wall)other;

				var last = _last;
				var current = _camera.GetPosition();

				// TODO: resetting position feels wrong because player gets "stuck" in walls. Instead find the maximum distance the player can walk towards a wall and position him there
				_camera.SetPosition(last);
			}
			else
			{
				throw new NotSupportedException("Currently player only supports collision with walls. Add collision logic for other objects here.");
			}
		}

		public void Render(IRenderContext renderContext, GameTime dt)
		{
		}

		public void Update(KeyboardManager keyboard, MouseManager mouse, GameTime dt)
		{
			_last = _camera.GetPosition();
		}

		#endregion
	}
}