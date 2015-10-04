namespace Engine.Rendering.Meshes
{

	#region Enumerations

	public enum DynamicMeshUsage
	{
		/// <summary>
		/// If you need to update the mesh very often, use this.
		/// It will use DrawUserPrimitives.
		/// For dynamic geometry (which is created on the fly e.g. each time it is drawn), you should use DrawUserPrimitives, however. This is able to do the copy in a more efficient way than if you manually created a vertex buffer, SetData into it, and then drew from that.
		/// </summary>
		UpdateOften,

		/// <summary>
		/// If you need to update the mesh only once in every while, use this.
		/// It will use DrawPrimitives.
		/// For persistent geometry (data that is drawn many times without changing), DrawPrimitives with a vertex buffer is dramatically faster.
		/// </summary>
		UpdateSeldom
	}

	#endregion
}