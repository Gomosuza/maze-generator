using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Engine.Datastructures.Quadtree
{
	/// <summary>
	/// A node contains 4 children (either further nodes or leaves) and may also contain elements (that are too big for child nodes).
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Node<T> : INode<T> where T : IQuadtreeElement
	{
		#region Fields

		private readonly INode<T>[] _children;
		private readonly List<T> _elements;
		private readonly INode<T> _parent;

		#endregion

		#region Constructors

		public Node(BoundingBox area, INode<T> parent, INode<T>[] children)
			: this(area, parent)
		{
			if (children == null || children.Length != 4)
			{
				throw new ArgumentException("there must be exactly four children.");
			}
			_children = children;
		}

		public Node(BoundingBox area, INode<T> parent)
		{
			BoundingBox = area;
			_parent = parent;
			_elements = new List<T>();
		}

		#endregion

		#region Properties

		public BoundingBox BoundingBox { get; }

		#endregion

		#region Methods

		public void Add(T element)
		{
			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}
			var child = _children.FirstOrDefault(c => c.BoundingBox.Contains(element.BoundingBox) == ContainmentType.Contains);
			if (child != null)
			{
				child.Add(element);
			}
			else
			{
				_elements.Add(element);
			}
		}

		public List<T> GetIntersectingElements(BoundingBox boundingBox)
		{
			if (!BoundingBox.Intersects(boundingBox))
			{
				return new List<T>();
			}
			var matches = _elements.Where(e => e.BoundingBox.Intersects(boundingBox)).ToList();
			var moreMatches = _children.SelectMany(c => c.GetIntersectingElements(boundingBox));

			matches.AddRange(moreMatches);
			return matches;
		}

		public List<T> GetIntersectingElements(BoundingFrustum frustum)
		{
			if (!BoundingBox.Intersects(frustum))
			{
				return new List<T>();
			}
			var matches = _elements.Where(e => e.BoundingBox.Intersects(frustum)).ToList();

			var moreMatches = _children.SelectMany(c => c.GetIntersectingElements(frustum));
			matches.AddRange(moreMatches);
			return matches;
		}

		public void Remove(T element)
		{
			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}
			// find node where element was added to
			var node = _children.FirstOrDefault(c => c.BoundingBox.Contains(element.BoundingBox) == ContainmentType.Contains);
			if (node != null)
			{
				node.Remove(element);
			}
			else
			{
				_elements.Remove(element);
			}
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