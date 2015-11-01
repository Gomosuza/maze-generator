using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Engine.Datastructures.Quadtree
{
	/// <summary>
	/// Generic quadtree implemementation that can grow past its initial bounds.
	/// </summary>
	public class Quadtree<T>
		where T : IQuadtreeElement
	{
		#region Constructors

		/// <summary>
		/// Creates a new instance of the quadtree in the given area.
		/// </summary>
		/// <param name="area">Initial area. If objects outside it are added the quadtree is automatically increased.</param>
		public Quadtree(BoundingBox area)
		{
			Root = new Leaf<T>(area, null);
		}

		#endregion

		#region Properties

		public INode<T> Root { get; }

		#endregion

		#region Methods

		public void Add(T element)
		{
			Root.Add(element);
		}

		/// <summary>
		/// Returns the set of elements that intersects with or are contained by the specific bounding box.
		/// </summary>
		/// <param name="boundingBox"></param>
		/// <returns></returns>
		public IEnumerable<T> GetIntersectingElements(BoundingBox boundingBox)
		{
			return Root.GetIntersectingElements(boundingBox);
		}

		/// <summary>
		/// Returns all elements that intersect with or are contained by the frustum.
		/// </summary>
		/// <param name="frustum"></param>
		/// <returns></returns>
		public IEnumerable<T> GetIntersectingElements(BoundingFrustum frustum)
		{
			return Root.GetIntersectingElements(frustum);
		}

		public void Remove(T element)
		{
			Root.Remove(element);
		}

		#endregion
	}
}