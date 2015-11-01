using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Engine.Datastructures.Quadtree
{
	/// <summary>
	/// Generic quadtree implemementation that.
	/// </summary>
	public class Quadtree<T>
		where T : IQuadtreeElement
	{
		#region Constructors

		/// <summary>
		/// Creates a new instance of the quadtree in the given area.
		/// </summary>
		/// <param name="area">Initial area. Objects outside it are not supported.</param>
		public Quadtree(BoundingBox area)
		{
			Root = new RootNode<T>(area);
		}

		#endregion

		#region Properties

		public INode<T> Root { get; }

		#endregion

		#region Methods

		public void Add(T element)
		{
			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}
			if (Root.BoundingBox.Contains(element.BoundingBox) == ContainmentType.Disjoint)
			{
				throw new NotSupportedException("Object outside quadtree");
			}
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