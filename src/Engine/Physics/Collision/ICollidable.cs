using Microsoft.Xna.Framework;
using Engine.Datastructures.Quadtree;

namespace Engine.Physics.Collision
{
	/// <summary>
	/// Describes a physics object that can collide with others.
	/// </summary>
	public interface ICollidable : IQuadtreeElement
	{
		#region Properties

		/// <summary>
		/// A bounding box that must contain the collidable object at all times.
		/// It will be used in the first level of collision tests.
		/// </summary>
		new BoundingBox BoundingBox { get; }

		/// <summary>
		/// Returns true if the collision object is static in the world and will never move, otherwise returns false.
		/// </summary>
		bool IsStatic { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Called to check whether the current instance collides with the other instance.
		/// This is only called once the <see cref="BoundingBox"/> check indicated that collision has occured
		/// and is intended to check the exact meshes for collision. 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		bool Collides(ICollidable other);

		#endregion
	}
}