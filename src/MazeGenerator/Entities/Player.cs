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
			var wall = other as Wall;
			if (wall != null)
			{
				var current = _camera.GetPosition();

				var bbox = wall.BoundingBox;

				// player must be somewhere between _last and current but current could be inside bbox, so clamp
				var updated = current;

				const float minDistanceFromWall = 1f;

				if (_last.X <= bbox.Min.X - minDistanceFromWall && current.X >= bbox.Min.X - minDistanceFromWall)
				{
					// player was on left and moved right
					updated.X = bbox.Min.X - minDistanceFromWall;
				}
				else if (_last.X >= bbox.Max.X + minDistanceFromWall && current.X <= bbox.Max.X + minDistanceFromWall)
				{
					// player was on right and moved left
					updated.X = bbox.Max.X + minDistanceFromWall;
				}
				if (_last.Z <= bbox.Min.Z - minDistanceFromWall && current.Z >= bbox.Min.Z - minDistanceFromWall)
				{
					// player was in front and moved back
					updated.Z = bbox.Min.Z - minDistanceFromWall;
				}
				else if (_last.Z >= bbox.Max.Z + minDistanceFromWall && current.Z <= bbox.Max.Z + minDistanceFromWall)
				{
					// player was back and moved forward
					updated.Z = bbox.Max.Z + minDistanceFromWall;
				}
				_camera.SetPosition(updated);
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