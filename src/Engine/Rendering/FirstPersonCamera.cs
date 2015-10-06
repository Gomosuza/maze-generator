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
			/// In person mode, the camera will be "locked" to the ground. If the player presses forward, it will not alter his vertical position.
			/// </summary>
			Person
		}

		#endregion

		#region Fields

		private const float NearZ = 0.5f;

		private readonly GraphicsDevice _device;

		private bool _dirty;
		private float _leftRightLeftRightRotation;
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
			FarZ = farZ;

			Position = initialPosition;
			Mode = mode;
			_dirty = true;
		}

		#endregion

		#region Properties

		public float FarZ { get; }

		/// <summary>
		/// The horizontal rotation around the Y axis.
		/// </summary>
		public float LeftRightRotation
		{
			get { return _leftRightLeftRightRotation; }
			private set
			{
				_leftRightLeftRightRotation = value;
				_dirty = true;
			}
		}

		/// <summary>
		/// Gets the mode of the camera.
		/// </summary>
		public FirstPersonMode Mode { get; }

		public Vector3 Position { get; private set; }

		/// <see cref="ICamera.Projection"/>
		public Matrix Projection { get; private set; }

		/// <summary>
		/// The vertical rotation around the X axis.
		/// </summary>
		public float UpDownRotation
		{
			get { return _upDownRotation; }
			private set
			{
				_upDownRotation = MathHelper.Clamp(value, -MathHelper.PiOver2, MathHelper.PiOver2);
				_dirty = true;
			}
		}

		/// <see cref="ICamera.View"/>
		public Matrix View { get; private set; }

		#endregion

		#region Methods

		/// <see cref="ICamera.DistanceToCamera"/>
		public float DistanceToCamera(Vector3 world) => (world - Position).Length();

		/// <see cref="ICamera.DistanceToCameraSquared"/>
		public float DistanceToCameraSquared(Vector3 world) => (world - Position).LengthSquared();

		/// <summary>
		/// Returns the current position of the camera.
		/// </summary>
		/// <returns></returns>
		public Vector3 GetPosition()
		{
			return Position;
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
			LeftRightRotation -= value;

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
					Matrix cameraRotation = Matrix.CreateRotationX(_upDownRotation) * Matrix.CreateRotationY(_leftRightLeftRightRotation);
					Vector3 rotatedVector = Vector3.Transform(movement, cameraRotation);
					Position += rotatedVector;
				}
					break;
				case FirstPersonMode.Person:
				{
					// restrict movement around Y axis
					Matrix cameraRotation = Matrix.CreateRotationY(_leftRightLeftRightRotation);
					Vector3 rotatedVector = Vector3.Transform(movement, cameraRotation);
					Position += rotatedVector;
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
			LeftRightRotation = verticalRotation;

			Update();
		}

		private void Update()
		{
			if (!_dirty)
			{
				return;
			}

			var rotation = Matrix.CreateRotationX(_upDownRotation) * Matrix.CreateRotationY(_leftRightLeftRightRotation);

			var rotated = Vector3.Transform(Vector3.UnitZ * -1, rotation);
			var final = Position + rotated;

			var up = Vector3.Transform(Vector3.Up, rotation);
			View = Matrix.CreateLookAt(Position, final, up);

			Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, _device.Viewport.AspectRatio, NearZ, FarZ);

			_dirty = false;
		}

		#endregion
	}
}