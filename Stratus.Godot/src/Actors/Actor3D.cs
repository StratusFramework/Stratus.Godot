using Godot;

using Stratus.Collections;
using Stratus.Extensions;
using Stratus.Godot.Extensions;
using Stratus.Models.Actors;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Stratus.Godot.Actors
{
	/// <summary>
	/// Handles movement and basic actions for an <see cref="IActor3D"/> such as interacting with other 
	/// <see cref="IObject"/>.
	/// </summary>
	public partial class Actor3D : CharacterBody3D, IActor3D
	{
		#region Export Fields
		[Export]
		public int speed { get; set; } = 5;
		[Export]
		public int acceleration { get; set; } = 3;
		[Export]
		public int fallAcceleration { get; set; } = 75;
		[Export]
		public Area3D area;
		[Export]
		public StateMachineRig3D rig;
		[Export]
		public float sprintMultiplier = 2;
		#endregion

		#region Private Fields
		private ArrayNavigator<Object3D> _objectsInRange = new();
		private Vector3? direction;
		private Node3D pivot;
		private Vector3 previousVelocity;
		private Func<Vector3> getInput;
		private bool receivedInput;
		#endregion

		#region Properties
		/// <summary>
		/// IF true, will affect the base speed by a multiplier
		/// </summary>
		public bool sprint { get; set; }
		public string name => Name;
		public System.Numerics.Vector3 position => Position.ToSystemVector3();
		public bool pollingInput => getInput != null;
		/// <summary>
		/// When using <see cref="Interact"/>, will change
		/// </summary>
		/// <remarks>Reset whenever the interactives in range change</remarks>
		public IArrayNavigator<Object3D> objectsInRange => _objectsInRange;
		#endregion

		#region Messages
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

		public override void _PhysicsProcess(double delta)
		{
			const float stoppingSpeed = 1f;

			if (pollingInput)
			{
				Move(getInput());
			}
			else
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
		#endregion


		#region Interface
		/// <summary>
		/// When set, will poll for the movement input internally by using the provider function.
		/// </summary>
		/// <param name="getInput"></param>
		public void Poll(Func<Vector3> getInput)
		{
			this.getInput = getInput;
		}

		/// <summary>
		/// Moves the actor in the given direction
		/// </summary>
		/// <param name="value"></param>
		public void Move(Vector3 value)
		{
			if (value == Vector3.Zero)
			{
				direction = value;
			}
			else
			{
				direction = value.Normalized();
				pivot.LookAt(Position + direction.Value, Vector3.Up);
			}

			receivedInput = true;
		}

		/// <summary>
		/// Toggles sprinting for the actor
		/// </summary>
		/// <param name="toggle"></param>
		public void ToggleSprint(bool toggle)
		{
			sprint = toggle;
		}

		/// <summary>
		/// Interacts with one of the <see cref="IObject"/> in range
		/// </summary>
		/// <param name="index"></param>
		public void Interact()
		{
			if (objectsInRange.valid)
			{
				rig.Set(DefaultAnimation.Interact);
				this.Log($"Interacting with {objectsInRange.current.name}");
				objectsInRange.current.Interact(this);
			}
		}
		#endregion

		#region Procedures
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
				//this.Log($"Collided with object {object3d.Name}");
			}
			else
			{
				//this.Log($"Collided with {node.Name}");
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
			UpdateObjectsInRange();
		}

		private void Area_BodyEntered(Node3D body)
		{
			this.Log($"{body} has entered my area");
			UpdateObjectsInRange();
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

			if (Velocity.Length() > 0)
			{
				rig.Set(sprint ? DefaultAnimation.Run : DefaultAnimation.Walk);
			}
			else
			{
				rig.Set(DefaultAnimation.Idle);
			}
		}

		private void UpdateObjectsInRange()
		{
			var bodies = area.GetOverlappingBodies();
			var objects = bodies.Select(b => b.GetParentOfType<Object3D>()).Where(o => o != null).ToArray();
			_objectsInRange.Set(objects);
			if (_objectsInRange.valid)
			{
				this.Log($"Can interact with {_objectsInRange.values.ToStringJoin(",", StratusStringEnclosure.SquareBracket, o => o.name)}");
			}
		}
		#endregion
	}
}