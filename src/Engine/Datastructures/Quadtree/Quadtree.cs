using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Engine.Datastructures.Quadtree
{
	/// <summary>
	/// Generic quadtree implemementation.
	/// </summary>
	public class Quadtree<T>
		where T : IQuadtreeElement
	{
		#region Fields

		private readonly INode<T> _root;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance of the quadtree in the given area.
		/// </summary>
		/// <param name="area">All content of this quad tree must be part of the bounding box. Objects outside are not allowed.</param>
		public Quadtree(BoundingBox area)
		{
			_root = new Leaf<T>(area, null);
		}

		#endregion

		#region Methods

		public void Add(T element)
		{
			_root.Add(element);
		}

		/// <summary>
		/// Returns the set of elements that intersects with the specific bounding box.
		/// </summary>
		/// <param name="boundingBox"></param>
		/// <returns></returns>
		public IEnumerable<T> GetIntersectingElements(BoundingBox boundingBox)
		{
			throw new System.NotImplementedException();
		}

		public void Remove(T element)
		{
			_root.Remove(element);
		}

		#endregion
	}
}