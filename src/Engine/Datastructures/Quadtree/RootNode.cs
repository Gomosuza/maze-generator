using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Engine.Datastructures.Quadtree
{
	internal class RootNode<T> : INode<T> where T : IQuadtreeElement
	{
		#region Fields

		private INode<T> _root;

		#endregion

		#region Constructors

		public RootNode(BoundingBox bbox)
		{
			BoundingBox = bbox;
			_root = new Leaf<T>(bbox, this);
		}

		#endregion

		#region Properties

		public BoundingBox BoundingBox { get; }

		#endregion

		#region Methods

		public void Add(T element)
		{
			_root.Add(element);
		}

		public IEnumerable<T> GetIntersectingElements(BoundingBox boundingBox)
		{
			return _root.GetIntersectingElements(boundingBox);
		}

		public IEnumerable<T> GetIntersectingElements(BoundingFrustum frustum)
		{
			return _root.GetIntersectingElements(frustum);
		}

		public void Remove(T element)
		{
			_root.Remove(element);
		}

		public void ReplaceChild(INode<T> node, Node<T> newNode)
		{
			if (_root != node)
			{
				throw new NotSupportedException("Invalid node to be replaced.");
			}
			_root = newNode;
		}

		#endregion
	}
}