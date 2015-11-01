using Microsoft.Xna.Framework;
using Engine.Physics.Collision;

namespace MazeGenerator.Entities
{
	/// <summary>
	/// Represents a piece of wall for collision purpose.
	/// </summary>
	public class Wall : ICollidable
	{
		#region Constructors

		public Wall(BoundingBox bbox)
		{
			BoundingBox = bbox;
		}

		#endregion

		#region Properties

		public bool IsStatic => true;

		public BoundingBox BoundingBox { get; }

		#endregion

		#region Methods

		public bool Collides(ICollidable other)
		{
			return other.Collides(this);
		}

		public void CollisionResponse(ICollidable other)
		{
		}

		#endregion
	}
}