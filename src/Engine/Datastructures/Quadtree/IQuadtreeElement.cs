using Microsoft.Xna.Framework;

namespace Engine.Datastructures.Quadtree
{
	/// <summary>
	/// The basic element of the quadtree.
	/// Needed to check where the element belongs.
	/// </summary>
	public interface IQuadtreeElement
	{
		#region Properties

		/// <summary>
		/// The bounding box for the current object.
		/// Since quadtrees are static by design the boundign box should not change for an object once it was added.
		/// If you really need to move the object, consider removing it from the tree, altering the boundingbox
		/// and adding it again, though this is only recommended if you don't alter it's bounding box often.
		/// </summary>
		BoundingBox BoundingBox { get; }

		#endregion
	}
}