namespace Engine.Datastructures.Quadtree
{
	/// <summary>
	/// Generic implementation of a quadtree node.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface INode<in T> : IQuadtreeElement
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
	}
}