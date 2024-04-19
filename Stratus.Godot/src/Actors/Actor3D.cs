using Godot;

using Stratus.Godot.Actors;
using Stratus.Godot.Extensions;
using Stratus.Models.Actors;

using System;

namespace Stratus.Godot.Actors
{
	public partial class Actor3D : CharacterBody3D, IActor3D
	{
		[Export]
		public int speed { get; set; } = 10;
		[Export]
		public int acceleration { get; set; } = 5;
		[Export]
		public int fallAcceleration { get; set; } = 75;
		[Export]
		public Area3D area;
		[Export]
		public StateMachineRig3D rig;
		[Export]
		public float sprintMultiplier = 2;

		/// <summary>
		/// IF true, will affect the base speed by a multiplier
		/// </summary>
		public bool sprint { get; set; }

		public System.Numerics.Vector3 position => Position.ToSystemVector3();
		public string name => Name;

		private Vector3? direction;
		private Node3D pivot;
		private bool receivedInput;
		private Vector3 previousVelocity;

		public override void _Ready()
		{
			base._Ready();
			pivot = GetNode<Node3D>("Pivot");

			if (area != null)
			{
				SetupDetection();
			}
			if (rig != null)
			{
			}
		}

		public void ToggleSprint(bool toggle) => sprint = toggle;

		public void Move(Vector3 value)
		{
			if (value == Vector3.Zero)
			{
				return;
			}

			direction = value.Normalized();
			pivot.LookAt(Position + direction.Value, Vector3.Up);
			receivedInput = true;
		}

		public override void _PhysicsProcess(double delta)
		{
			const float stoppingSpeed = 1f;

			/**
			 *
			 * 	# We separate out the y velocity to not interpolate on the gravity
				var y_velocity := velocity.y
				velocity.y = 0.0
				velocity = velocity.lerp(_move_direction * move_speed, acceleration * delta)
				if _move_direction.length() == 0 and velocity.length() < stopping_speed:
					velocity = Vector3.ZERO
				velocity.y = y_velocity
			 *
			 */

			if (!receivedInput)
			{
				// Exit early
				if (Velocity == Vector3.Zero)
				{
					return;
				}

				direction = Vector3.Zero;
			}

			previousVelocity = Velocity;

			float delta_f = (float)delta;
			float targetSpeed = sprint ? sprintMultiplier * speed : speed;
			var newVelocity = Velocity.Lerp(direction.Value * targetSpeed, acceleration * delta_f);
			newVelocity.Y = 0;
			if (direction.Value.Length() == 0 && newVelocity.Length() < stoppingSpeed)
			{
				newVelocity = Vector3.Zero;
			}

			Velocity = newVelocity;
			receivedInput = false;

			bool collided = MoveAndSlide();
			if (collided)
			{
				OnSlideCollision();
			}

			UpdateAnimation();
		}

		private void OnSlideCollision()
		{
			var collision = GetLastSlideCollision();
			var node = ((Node)(collision.GetCollider()));
			TryGetInteractive(node);
		}

		private void TryGetInteractive(Node node)
		{
			var object3d = node.GetParentOfType<Object3D>();
			if (object3d != null)
			{
				this.Log($"Collided with object {object3d.Name}");
			}
			else
			{
				this.Log($"Collided with {node.Name}");
			}
		}

		private void SetupDetection()
		{
			area.BodyEntered += this.Area_BodyEntered;
			area.BodyExited += this.Area_BodyExited;
		}

		private void Area_BodyExited(Node3D body)
		{
			this.Log($"{body} has exited my area");
			NotifyObjects();
		}

		private void Area_BodyEntered(Node3D body)
		{
			this.Log($"{body} has entered my area");
			NotifyObjects();
		}

		private void UpdateAnimation()
		{
			if (rig == null)
			{
				return;
			}

			if (previousVelocity == Velocity)
			{
				return;
			}

			// ALso handle run..
			if (Velocity.Length() > 0)
			{
				rig.Set(sprint ? DefaultAnimation.Run : DefaultAnimation.Walk);
			}
			else
			{
				rig.Set(DefaultAnimation.Idle);
			}
		}

		private void NotifyObjects()
		{
			var bodies = area.GetOverlappingBodies();
		}
	}
}