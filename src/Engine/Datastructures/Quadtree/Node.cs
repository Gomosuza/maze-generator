using System.Linq;
using Microsoft.Xna.Framework;

namespace Engine.Datastructures.Quadtree
{
	public class Node<T> : INode<T>
		where T : IQuadtreeElement
	{
		#region Fields

		private INode<T>[] _children;

		#endregion

		#region Constructors

		public Node(BoundingBox area, INode<T> parent)
		{
			BoundingBox = area;
		}

		#endregion

		#region Properties

		public BoundingBox BoundingBox { get; }

		#endregion

		#region Methods

		public void Add(T element)
		{
			var child = _children.FirstOrDefault(c => c.BoundingBox.Contains(element.BoundingBox) == ContainmentType.Contains);
			if (child != null)
			{
				child.Add(element);
			}
			else
			{
				// TODO: add to current node, as no child can contain it
			}
		}

		public void Remove(T element)
		{
			var children = _children.Where(c => c.BoundingBox.Intersects(element.BoundingBox)).ToList();
			foreach (var c in children)
			{
				c.Remove(element);
			}
			if (!children.Any())
			{
				// TODO: remove from current node.
			}
		}

		#endregion
	}
}