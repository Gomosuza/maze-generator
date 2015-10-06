using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Input;
using Engine.Rendering.Brushes;
using Engine.Rendering.Meshes;

namespace Engine.Rendering.Components
{
	/// <summary>
	/// Components that draws 3 lines for the axis (red = x, green = y, blue = z axis).
	/// </summary>
	public class GridAxis : IComponent
	{
		#region Fields

		private readonly Mesh _axis;
		private readonly bool _projectCurrentPositionOntoLines;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance of the grid axis.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="projectCurrentPositionOntoLines">If true Position will be drawn onto the grid.</param>
		public GridAxis(IRenderContext context, bool projectCurrentPositionOntoLines)
		{
			_projectCurrentPositionOntoLines = projectCurrentPositionOntoLines;
			const int distance = 10000;
			var vertices = new[]
			{
				new VertexPositionColor(Vector3.UnitX * distance, Color.Red),
				new VertexPositionColor(Vector3.UnitX * -distance, Color.Red),
				new VertexPositionColor(Vector3.UnitY * distance, Color.Green),
				new VertexPositionColor(Vector3.UnitY * -distance, Color.Green),
				new VertexPositionColor(Vector3.UnitZ * distance, Color.Blue),
				new VertexPositionColor(Vector3.UnitZ * -distance, Color.Blue)
			};
			_axis = context.MeshCreator.CreateMesh(PrimitiveType.LineList, vertices);
		}

		#endregion

		#region Methods

		public void Render(IRenderContext renderContext, GameTime dt)
		{
			renderContext.RenderContext3D.DrawMesh(_axis, Matrix.Identity, VertexColorBrush.Default);
			if (_projectCurrentPositionOntoLines)
			{
				var p = renderContext.RenderContext3D.Camera.GetPosition();
				var xPos = new Vector3(p.X, 0, 0);
				var yPos = new Vector3(0, p.Y, 0);
				var zPos = new Vector3(0, 0, p.Z);
				var x = renderContext.RenderContext3D.Camera.WorldToScreen(xPos);
				var y = renderContext.RenderContext3D.Camera.WorldToScreen(yPos);
				var z = renderContext.RenderContext3D.Camera.WorldToScreen(zPos);

				renderContext.RenderContext2D.DrawString($"X: {p.X:0.00}", x, Color.Red, FontSize.Tiny);
				renderContext.RenderContext2D.DrawString($"Y: {p.Y:0.00}", y, Color.Green, FontSize.Tiny);
				renderContext.RenderContext2D.DrawString($"Z: {p.Z:0.00}", z, Color.Blue, FontSize.Tiny);
			}
		}

		public void Update(KeyboardManager keyboard, MouseManager mouse, GameTime dt)
		{
		}

		#endregion
	}
}