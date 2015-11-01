using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Engine.Datastructures.Quadtree
{
	/// <summary>
	/// A leaf marks the end of a quadtree chain.
	/// It will contain a list of all elements within its area.
	/// </summary>
	public class Leaf<T> : INode<T>
		where T : IQuadtreeElement
	{
		#region Fields

		/// <summary>
		/// The maximum amount of objects, before the leaf is replaced by subnodes.
		/// </summary>
		private const int MaxElementCount = 50;

		private readonly List<T> _elements;
		private readonly INode<T> _parent;

		#endregion

		#region Constructors

		public Leaf(BoundingBox area, INode<T> parent)
		{
			if (parent == null)
			{
				throw new ArgumentNullException(nameof(parent));
			}
			if (parent is Leaf<T>)
			{
				throw new NotSupportedException("parent of a leaf cannot be a leaf");
			}
			_elements = new List<T>();
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
			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}
			_elements.Add(element);
			if (_elements.Count > MaxElementCount)
			{
				// split into 4 children
				var children = new INode<T>[4];
				var min = BoundingBox.Min;
				var center = (BoundingBox.Min + BoundingBox.Max) / 2f;
				var max = BoundingBox.Max;
				var topLeft = new BoundingBox(new Vector3(min.X, min.Y, min.Z), new Vector3(center.X, max.Y, center.Z));
				var topRight = new BoundingBox(new Vector3(center.X, min.Y, min.Z), new Vector3(max.X, max.Y, center.Z));
				var bottomLeft = new BoundingBox(new Vector3(min.X, min.Y, center.Z), new Vector3(center.X, max.Y, max.Z));
				var bottomRight = new BoundingBox(new Vector3(center.X, min.Y, center.Z), new Vector3(max.X, max.Y, max.Z));
				var newNode = new Node<T>(BoundingBox, _parent, children);
				children[0] = new Leaf<T>(topLeft, newNode);
				children[1] = new Leaf<T>(topRight, newNode);
				children[2] = new Leaf<T>(bottomLeft, newNode);
				children[3] = new Leaf<T>(bottomRight, newNode);
				foreach (var e in _elements)
				{
					newNode.Add(e);
				}
				var node = _parent as Node<T>;
				if (node != null)
				{
					node.ReplaceChild(this, newNode);
				}
				else
				{
					((RootNode<T>)_parent).ReplaceChild(this, newNode);
				}
			}
		}

		public List<T> GetIntersectingElements(BoundingBox boundingBox)
		{
			if (!BoundingBox.Intersects(boundingBox))
			{
				return new List<T>();
			}
			return _elements.Where(e => e.BoundingBox.Intersects(boundingBox)).ToList();
		}

		public List<T> GetIntersectingElements(BoundingFrustum frustum)
		{
			if (!BoundingBox.Intersects(frustum))
			{
				return new List<T>();
			}
			return _elements.Where(e => e.BoundingBox.Intersects(frustum)).ToList();
		}

		public void Remove(T element)
		{
			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}
			_elements.Remove(element);
		}

		#endregion
	}
}