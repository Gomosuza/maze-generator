using System;
using MazeGenerator.Generator;
using Microsoft.Xna.Framework;
using Engine.Physics.Collision;

namespace MazeGenerator.Entities
{
	/// <summary>
	/// Represents a piece of wall for a single cell for collision purpose.
	/// </summary>
	public class Wall : ICollidable
	{
		#region Fields

		private readonly Cell _cell;

		#endregion

		#region Constructors

		public Wall(Cell c)
		{
			if (c == null)
			{
				throw new ArgumentNullException(nameof(c));
			}
			if (c.Mode != CellMode.Wall)
			{
				throw new NotSupportedException();
			}
			_cell = c;
		}

		#endregion

		#region Properties

		public bool IsStatic => true;

		public BoundingBox BoundingBox => _cell.GetBoundingBox();

		#endregion

		#region Methods

		public bool Collides(ICollidable other)
		{
			return other.Collides(this);
		}

		public void CollisionResponse(ICollidable other)
		{
		}

		#endregion
	}
}