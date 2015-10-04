using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering
{
	/// <summary>
	/// Basic camera implementation.
	/// </summary>
	public sealed class Camera : ICamera
	{
		#region Fields

		private const float NearZ = 0.5f;

		private readonly GraphicsDevice _device;
		private readonly float _farZ;

		private bool _dirty;
		private Vector3 _lookAt;
		private Vector3 _position;

		#endregion

		#region Constructors

		private Camera(GraphicsDevice device, Vector3 initialPosition, Vector3 lookAt, float farZ = 1000)
		{
			_device = device;
			_dirty = true;
			_farZ = farZ;

			// bypass property as it would just edit the other value as well, we don't want that here
			_position = initialPosition;
			_lookAt = lookAt;
		}

		#endregion

		#region Properties

		public Vector3 LookAt
		{
			get { return _lookAt; }
			set
			{
				if (_lookAt == value)
				{
					return;
				}

				var delta = _lookAt - value;
				_position += delta;
				_lookAt = value;
				_dirty = true;
			}
		}

		public Vector3 Position
		{
			get { return _position; }
			set
			{
				if (_position == value)
				{
					return;
				}

				// ensure the camera distance is maintained
				var delta = _position - value;
				_lookAt += delta;
				_position = value;
				_dirty = true;
			}
		}

		public Matrix Projection { get; private set; }

		public Matrix View { get; private set; }

		#endregion

		#region Methods

		public float DistanceToCamera(Vector3 world) => (world - Position).Length();

		public float DistanceToCameraSquared(Vector3 world) => (world - Position).LengthSquared();

		public Ray ScreenToWorld(Vector2 screen)
		{
			throw new NotImplementedException();
		}

		public void Update(GameTime dt) => Update();

		public Vector2 WorldToScreen(Vector3 world)
		{
			Update();

			Vector3 tmp = _device.Viewport.Project(world, Projection, View, Matrix.Identity);
			return new Vector2(tmp.X, tmp.Y);
		}

		/// <summary>
		/// Creates a camera at the given position that looks at the specific destination .
		/// </summary>
		/// <returns></returns>
		public static ICamera CreateLookAt(GraphicsDevice device, Vector3 position, Vector3 lookAt, float farZ = 1000)
		{
			return new Camera(device, position, lookAt, farZ);
		}

		private void Update()
		{
			if (!_dirty)
			{
				return;
			}

			View = Matrix.CreateLookAt(Position, LookAt, Vector3.Up);
			Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, _device.Viewport.AspectRatio, NearZ, _farZ);

			_dirty = false;
		}

		#endregion
	}
}