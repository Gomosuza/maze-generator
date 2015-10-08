using System.Collections.Generic;
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
		/// The maximum amount of objects, before the lead is replaced by subnodes.
		/// </summary>
		private const int MaxElementCount = 50;

		private readonly List<T> _elements;
		private readonly INode<T> _parent;

		#endregion

		#region Constructors

		public Leaf(BoundingBox area, INode<T> parent)
		{
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
			_elements.Add(element);
			if (_elements.Count > MaxElementCount)
			{
				var children = new Node<T>[4];
				// TODO: scatter elements across children
				//_parent.ReplaceChild(this, new Node<T>(children));
			}
		}

		public void Remove(T element)
		{
			_elements.Remove(element);
		}

		#endregion
	}
}