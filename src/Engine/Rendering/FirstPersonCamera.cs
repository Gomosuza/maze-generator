using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering
{
	/// <summary>
	/// Basic camera implementation.
	/// </summary>
	public sealed class FirstPersonCamera : ICamera
	{
		#region Enumerations

		public enum FirstPersonMode
		{
			/// <summary>
			/// In plane mode, the camera will fly directly in the direction it is pointed.
			/// This may e.g. result in the camera flying straight up when forward is pressed and the user is looking straight up.
			/// </summary>
			Plane,

			/// <summary>
			/// In person mode, the camera will be "locked" to the ground. If the player presses forward, it will no alter his vertical position.
			/// </summary>
			Person
		}

		#endregion

		#region Fields

		private const float NearZ = 0.5f;

		private readonly GraphicsDevice _device;
		private readonly float _farZ;

		private bool _dirty;
		private float _leftRightRotation;
		private Vector3 _position;
		private float _upDownRotation;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance of the camera.
		/// The camera will face negative Z direction by default.
		/// </summary>
		/// <param name="device"></param>
		/// <param name="initialPosition">The starting point of the camera.</param>
		/// <param name="mode"></param>
		/// <param name="farZ">The far clipping plane.</param>
		public FirstPersonCamera(GraphicsDevice device, Vector3 initialPosition, FirstPersonMode mode, float farZ = 1000)
		{
			_device = device;
			_farZ = farZ;

			_position = initialPosition;
			Mode = mode;
			_dirty = true;
		}

		#endregion

		#region Properties

		public FirstPersonMode Mode { get; }

		public Vector3 Position
		{
			get { return _position; }
		}

		/// <see cref="ICamera.Projection"/>
		public Matrix Projection { get; private set; }

		/// <see cref="ICamera.View"/>
		public Matrix View { get; private set; }

		private float RotationY
		{
			get { return _leftRightRotation; }
			set
			{
				if (Math.Abs(_leftRightRotation - value) < float.Epsilon)
				{
					return;
				}
				_leftRightRotation = value;
				_dirty = true;
			}
		}

		private float UpDownRotation
		{
			get { return _upDownRotation; }
			set
			{
				if (Math.Abs(_upDownRotation - value) < float.Epsilon)
				{
					return;
				}
				_upDownRotation = MathHelper.Clamp(value, -MathHelper.PiOver2, MathHelper.PiOver2);
				_dirty = true;
			}
		}

		#endregion

		#region Methods

		/// <see cref="ICamera.DistanceToCamera"/>
		public float DistanceToCamera(Vector3 world) => (world - Position).Length();

		/// <see cref="ICamera.DistanceToCameraSquared"/>
		public float DistanceToCameraSquared(Vector3 world) => (world - Position).LengthSquared();

		public Vector3 GetPosition()
		{
			return _position;
		}

		/// <see cref="ICamera.ScreenToWorld"/>
		public Ray ScreenToWorld(Vector2 screen)
		{
			Update();

			var vector = _device.Viewport.Unproject(new Vector3(screen, 0), Projection, View, Matrix.Identity);
			vector.Normalize();
			return new Ray(Position, vector);
		}

		/// <see cref="ICamera.Update"/>
		public void Update(GameTime dt) => Update();

		/// <see cref="ICamera.WorldToScreen"/>
		public Vector2 WorldToScreen(Vector3 world)
		{
			Update();

			Vector3 tmp = _device.Viewport.Project(world, Projection, View, Matrix.Identity);
			return new Vector2(tmp.X, tmp.Y);
		}

		/// <summary>
		/// Adds the value to the horizontal rotation (if positive will make the player look right, if negative will make him look left).
		/// </summary>
		/// <param name="value"></param>
		public void AddHorizontalRotation(float value)
		{
			RotationY -= value;

			_dirty = true;

			Update();
		}

		/// <summary>
		/// Adds the value to the vertical rotation (if positive will make the player look upwards, if negative will make him look downwards).
		/// The rotation is automatically clamped at 0 and <see cref="MathHelper.Pi"/>.
		/// </summary>
		/// <param name="value"></param>
		public void AddVerticalRotation(float value)
		{
			UpDownRotation -= value;

			_dirty = true;

			Update();
		}

		public void Move(Vector3 movement)
		{
			switch (Mode)
			{
				case FirstPersonMode.Plane:
				{
					Matrix cameraRotation = Matrix.CreateRotationX(_upDownRotation) * Matrix.CreateRotationY(_leftRightRotation);
					Vector3 rotatedVector = Vector3.Transform(movement, cameraRotation);
					_position += rotatedVector;
				}
					break;
				case FirstPersonMode.Person:
				{
					// restrict movement around Y axis
					Matrix cameraRotation = Matrix.CreateRotationY(_leftRightRotation);
					Vector3 rotatedVector = Vector3.Transform(movement, cameraRotation);
					_position += rotatedVector;
				}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			_dirty = true;
		}

		/// <summary>
		/// Sets both rotation components.
		/// </summary>
		/// <param name="horizontalRotation">The rotation around the Y axis.</param>
		/// <param name="verticalRotation">The rotation around the X axis.</param>
		public void SetRotation(float horizontalRotation, float verticalRotation)
		{
			UpDownRotation = horizontalRotation;
			RotationY = verticalRotation;

			Update();
		}

		private void Update()
		{
			if (!_dirty)
			{
				return;
			}

			var rotation = Matrix.CreateRotationX(_upDownRotation) * Matrix.CreateRotationY(_leftRightRotation);

			var rotated = Vector3.Transform(Vector3.UnitZ * -1, rotation);
			var final = Position + rotated;

			var up = Vector3.Transform(Vector3.Up, rotation);
			View = Matrix.CreateLookAt(Position, final, up);

			Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, _device.Viewport.AspectRatio, NearZ, _farZ);

			_dirty = false;
		}

		#endregion
	}
}