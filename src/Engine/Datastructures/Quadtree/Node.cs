using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Engine.Datastructures.Quadtree
{
	public class Node<T> : INode<T>
		where T : IQuadtreeElement
	{
		#region Fields

		private readonly INode<T>[] _children;
		private readonly INode<T> _parent;

		#endregion

		#region Constructors

		public Node(BoundingBox area, INode<T> parent, INode<T>[] children) : this(area, parent)
		{
			if (children == null || children.Length != 4)
			{
				throw new ArgumentException("there must be exactly four children.");
			}
			_children = children;
		}

		public Node(BoundingBox area, INode<T> parent)
		{
			if (parent == null)
			{
				throw new ArgumentNullException(nameof(parent));
			}
			BoundingBox = area;
			_parent = parent;
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

		public IEnumerable<T> GetIntersectingElements(BoundingBox boundingBox)
		{
			return _children.SelectMany(c => c.GetIntersectingElements(boundingBox));
		}

		public void ReplaceChild(Leaf<T> find, Node<T> replacement)
		{
			var idx = Array.IndexOf(_children, find);
			if (idx == -1)
			{
				throw new InvalidOperationException("Element is not a child, cannot replace it.");
			}
			_children[idx] = replacement;
		}

		#endregion
	}
}