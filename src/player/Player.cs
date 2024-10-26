using Godot;
using System;
using Library;

public partial class Player : CharacterBody3D
{
	// Movement properties
	[Export] public float MaxSpeed = 9f;
	public float Acceleration = 0.085f;
	public float Deceleration = 0.3f;
	public float AirAcceleration = 0.1f;
	public float AirDeceleration = 0.03f;
	
	// Viewport properties
	[Export] public float LookSensitivity = 0.3f;
	public Camera3D Viewport;
	public Node3D ViewportPivot;

	// Jump properties
	[Export] public float JumpVelocity = 7f;
	public bool CanDoubleJump = true;
	public bool Airborne = false;
	[Signal] public delegate void LeftGroundEventHandler();
	[Signal] public delegate void LandedEventHandler();

	// Attack properties
	private Attack _basicAttack;
	public bool CanAttack = true;
	public ShapeCast3D AttackHitbox;
	public Timer AttackCooldown;

	// Dash properties
	private float _dashDuration = 0.25f;
	private float _dashCooldown = 1.0f;
	private float _dashImpulse = 25f;
	public Timer DashDurationTimer = new Timer();
	public Timer DashCooldownTimer = new Timer();
	[Signal] public delegate void DashingEventHandler();
	

    public override void _Ready()
    {
        InitAttacking();
		InitViewport();
		InitDashing();

		LeftGround += () => Airborne = true;
		Landed += () => {Airborne = false; CanDoubleJump = true;};
	}

    public override void _PhysicsProcess(double delta)
	{
		Vector2 inputDir = Vectors.GetCurrentInputDirection();
		Velocity = HandlePlayerMovement(inputDir, Transform, Velocity, Acceleration, delta);
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
				velocity.Y = JumpVelocity;
				EmitSignal(SignalName.LeftGround);
			}
			else if (!IsOnFloor() && CanDoubleJump)
			{
				velocity.Y = JumpVelocity;
				CanDoubleJump = false;
			}
			Velocity = velocity;
		}

		if (Input.IsActionJustPressed("dash") && DashCooldownTimer.IsStopped())
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
		if (!IsOnFloor() && DashDurationTimer.IsStopped())
		{
			targetVelocity += GetGravity() * (float)delta;
		}
		else
		{
			// If we were Airborne before touching the floor, emit our landing signal
			if (Airborne)
			{
				EmitSignal(SignalName.Landed);
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
			targetVelocity = Vectors.MoveTowardsXZ(targetVelocity, direction*MaxSpeed, acceleration);
		}
		// If no input direction, decelerate us based on if we are on a surface or not.
		else
		{
			// Ensure a duration of frictionless motion when we dash, dictated by DashDurationTimer, otherwise we decelerate.
			if (DashDurationTimer.IsStopped()) {
				if (IsOnFloor()) 
				{
					targetVelocity = Vectors.MoveTowardsXZ(targetVelocity, Vector3.Zero, Deceleration);
				}
				else
				{
					targetVelocity = Vectors.MoveTowardsXZ(targetVelocity, Vector3.Zero, AirDeceleration);
				}
			}
			
		}


		return targetVelocity;
	}

	private void InitAttacking()
	{
		_basicAttack = new Attack();
		AttackHitbox = GetNode<ShapeCast3D>("Hitbox");
		AttackCooldown = GetNode<Timer>("AttackCooldown");

		AttackCooldown.WaitTime = _basicAttack.cooldown;

		AttackCooldown.Timeout += () => CanAttack = true;
	}

	public void FireBasicAttack()
	{
		// Set _CanAttack to false and start the attack cooldown timer
		CanAttack = false;
		AttackCooldown.Start();
		if (AttackHitbox.IsColliding())
		{
			for(int collision = 0; collision < AttackHitbox.GetCollisionCount(); collision++)
			{
				var collider = AttackHitbox.GetCollider(collision) as Node;
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
		DashDurationTimer.OneShot = true;
		DashDurationTimer.WaitTime = _dashDuration;
		AddChild(DashDurationTimer);
		DashCooldownTimer.OneShot = true;
		DashCooldownTimer.WaitTime = _dashCooldown;
		AddChild(DashCooldownTimer);
	}

	private void PlayerDash(Vector2 inputDir, Transform3D curTransform)
	{
		// Vector3 velocityXZ = new Vector3(Velocity.X, 0, Velocity.Z);
		// Velocity = velocityXZ.Normalized() * _dashImpulse;

		// Get our desired direction normalized, then multiply that by our dash impulse to get our new initial dash velocity.
		Vector3 direction = (curTransform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		Vector3 targetVelocity = direction * _dashImpulse;
		Velocity = new Vector3(targetVelocity.X, 0, targetVelocity.Z);

		DashDurationTimer.Start();
		DashCooldownTimer.Start();
		EmitSignal(SignalName.Dashing);
	}

	private void InitViewport()
	{
		ViewportPivot = GetNode<Node3D>("CameraPivot");
		Viewport = GetNode<Camera3D>("Camera3D");
	}

}
