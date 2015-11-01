using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Engine.Datastructures.Quadtree
{
	/// <summary>
	/// Generic implementation of a quadtree node.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface INode<T> : IQuadtreeElement
		where T : IQuadtreeElement
	{
		#region Methods

		/// <summary>
		/// Add the element.
		/// </summary>
		/// <param name="element"></param>
		void Add(T element);

		/// <summary>
		/// Returns all elements that intersect with or are contained by the specific bounding box.
		/// </summary>
		/// <param name="boundingBox"></param>
		/// <returns></returns>
		IEnumerable<T> GetIntersectingElements(BoundingBox boundingBox);

		/// <summary>
		/// Returns all elements that intersect with or are contained by the frustum.
		/// </summary>
		/// <param name="frustum"></param>
		/// <returns></returns>
		IEnumerable<T> GetIntersectingElements(BoundingFrustum frustum);

		/// <summary>
		/// Remove the element.
		/// </summary>
		/// <param name="element"></param>
		void Remove(T element);

		#endregion
	}
}