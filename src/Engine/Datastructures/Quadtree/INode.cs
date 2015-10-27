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
		/// Remove the element.
		/// </summary>
		/// <param name="element"></param>
		void Remove(T element);

		#endregion

		IEnumerable<T> GetIntersectingElements(BoundingBox boundingBox);

		/// <summary>
		/// When called allows to replace one child with another one.
		/// </summary>
		/// <param name="find"></param>
		/// <param name="replacement"></param>
		void ReplaceChild(Leaf<T> find, Node<T> replacement);
	}
}