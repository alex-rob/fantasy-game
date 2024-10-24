using Godot;
using System;
using Library;

public partial class Player : CharacterBody3D
{
	// Movement properties
	[Export] public float max_speed = 9f;
	private float _acceleration = 0.085f;
	private float _deceleration = 0.3f;
	private float _airDeceleration = 0.03f;
	
	// Viewport properties
	[Export] public float lookSensitivity = 0.3f;
	public Camera3D viewport;
	public Node3D viewportPivot;

	// Jump properties
	[Export] public float jumpVelocity = 7f;
	public bool canDoubleJump = true;
	public bool airborne = false;
	[Signal] public delegate void leftGroundEventHandler();
	[Signal] public delegate void landedEventHandler();

	// Attack properties
	private Attack _basicAttack;
	public bool canAttack = true;
	public ShapeCast3D attackHitbox;
	public Timer attackCooldown;

	// Dash properties
	private float _dashDuration = 0.25f;
	private float _dashCooldown = 1.0f;
	private float _dashImpulse = 25f;
	private Timer _dashDurationTimer = new Timer();
	public Timer dashCooldownTimer = new Timer();
	[Signal] public delegate void dashingEventHandler();
	

    public override void _Ready()
    {
        InitAttacking();
		InitViewport();
		InitDashing();

		leftGround += () => airborne = true;
		landed += () => {airborne = false; canDoubleJump = true;};
	}

    public override void _PhysicsProcess(double delta)
	{
		Vector2 inputDir = Vectors.GetCurrentInputDirection();
		Velocity = HandlePlayerMovement(inputDir, Transform, Velocity, _acceleration, delta);
		MoveAndSlide();
	}

    public override void _Input(InputEvent @event)
    {
		// When we attempt to jump, check if we are on the floor or have double jump available
		if (Input.IsActionJustPressed("jump"))
		{
			Vector3 velocity = Velocity;
			if (IsOnFloor())
			{
				velocity.Y = jumpVelocity;
				EmitSignal(SignalName.leftGround);
			}
			else if (!IsOnFloor() && canDoubleJump)
			{
				velocity.Y = jumpVelocity;
				canDoubleJump = false;
			}
			Velocity = velocity;
		}

		if (Input.IsActionJustPressed("dash") && dashCooldownTimer.IsStopped())
		{
			Vector2 dir = Vectors.GetCurrentInputDirection();
			if (dir == Vector2.Zero) dir = Vector2.Up;
			PlayerDash(dir, Transform);
		}
    }

	private Vector3 HandlePlayerMovement(Vector2 inputDir, Transform3D curTransform, Vector3 curVelocity, float acceleration, double delta)
	{
		Vector3 targetVelocity = curVelocity;

		// Add the gravity.
		if (!IsOnFloor() && _dashDurationTimer.IsStopped())
		{
			targetVelocity += GetGravity() * (float)delta;
		}
		else
		{
			// If we were airborne before touching the floor, emit our landing signal
			if (airborne)
			{
				EmitSignal(SignalName.landed);
			}
		}

		// Get the input direction and handle the movement/deceleration.
		// Projects our input direction across our current transform basis to operate based on our existing transformation.
		// Thus, if the player is rotated, we will still be moving in the correct direction.
		Vector3 direction = (curTransform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		// If we are trying to move in a direction, try to move our velocity toward the target velocity
		// at the rate of our acceleration.
		if (direction != Vector3.Zero)
		{
			targetVelocity = Vectors.MoveTowardsXZ(targetVelocity, direction*max_speed, acceleration);
		}
		// If no input direction, decelerate us based on if we are on a surface or not.
		else
		{
			// Ensure a duration of frictionless motion when we dash, dictated by _dashDurationTimer, otherwise we decelerate.
			if (_dashDurationTimer.IsStopped()) {
				if (IsOnFloor()) 
				{
					targetVelocity = Vectors.MoveTowardsXZ(targetVelocity, Vector3.Zero, _deceleration);
				}
				else
				{
					targetVelocity = Vectors.MoveTowardsXZ(targetVelocity, Vector3.Zero, _airDeceleration);
				}
			}
			
		}


		return targetVelocity;
	}

	private void InitAttacking()
	{
		_basicAttack = new Attack();
		attackHitbox = GetNode<ShapeCast3D>("Hitbox");
		attackCooldown = GetNode<Timer>("AttackCooldown");

		attackCooldown.WaitTime = _basicAttack.cooldown;

		attackCooldown.Timeout += () => canAttack = true;
	}

	public void FireBasicAttack()
	{
		// Set _canAttack to false and start the attack cooldown timer
		canAttack = false;
		attackCooldown.Start();
		if (attackHitbox.IsColliding())
		{
			for(int collision = 0; collision < attackHitbox.GetCollisionCount(); collision++)
			{
				var collider = attackHitbox.GetCollider(collision) as Node;
				if (collider is Hurtbox && collider.IsInGroup("Enemies"))
				{
					Hurtbox target = (Hurtbox) collider;
					target.HitRegistered(_basicAttack);
				}
			}
		}
	}

	private void InitDashing()
	{
		_dashDurationTimer.OneShot = true;
		_dashDurationTimer.WaitTime = _dashDuration;
		AddChild(_dashDurationTimer);
		dashCooldownTimer.OneShot = true;
		dashCooldownTimer.WaitTime = _dashCooldown;
		AddChild(dashCooldownTimer);
	}

	private void PlayerDash(Vector2 inputDir, Transform3D curTransform)
	{
		// Vector3 velocityXZ = new Vector3(Velocity.X, 0, Velocity.Z);
		// Velocity = velocityXZ.Normalized() * _dashImpulse;

		// Get our desired direction normalized, then multiply that by our dash impulse to get our new initial dash velocity.
		Vector3 direction = (curTransform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		Vector3 targetVelocity = direction * _dashImpulse;
		Velocity = new Vector3(targetVelocity.X, 0, targetVelocity.Z);

		_dashDurationTimer.Start();
		dashCooldownTimer.Start();
		EmitSignal(SignalName.dashing);
	}

	private void InitViewport()
	{
		viewportPivot = GetNode<Node3D>("CameraPivot");
		viewport = GetNode<Camera3D>("Camera3D");
	}

}
