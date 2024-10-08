using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export] public float speed = 5.0f;
	[Export] public float jumpVelocity = 4.5f;
	[Export] public float lookSensitivity = 0.3f;
	public Attack basicAttack;
	public ShapeCast3D attackHitbox;
	public Timer attackCooldown;
	public bool airborne = false;

	private Camera3D _viewport;
	private Node3D _viewportPivot;
	private bool _canDoubleJump = true;
	private bool _canAttack = true;

	[Signal] public delegate void leftGroundEventHandler();
	[Signal] public delegate void landedEventHandler();

    public override void _Ready()
    {
		// Attack initialization
        basicAttack = new Attack();
		attackHitbox = GetNode<ShapeCast3D>("Hitbox");
		attackCooldown = GetNode<Timer>("AttackCooldown");

		attackCooldown.WaitTime = basicAttack.cooldown;

		attackCooldown.Timeout += () => _canAttack = true;

		// Viewport initialization
		_viewportPivot = GetNode<Node3D>("CameraPivot");
		_viewport = GetNode<Camera3D>("Camera3D");

		leftGround += () => airborne = true;
		landed += () => {airborne = false; _canDoubleJump = true;};
	}

    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

	if (Input.IsActionJustPressed("jump")) // TODO fix this up too. sorry sober alex
	{
		if (IsOnFloor() || (!IsOnFloor() && _canDoubleJump))
		{
			if(!IsOnFloor() && _canDoubleJump) _canDoubleJump = false;
			velocity.Y = jumpVelocity;
			if(IsOnFloor()) EmitSignal(SignalName.leftGround);
		}
	}
	if (IsOnFloor())
	{
		if (Input.IsActionJustPressed("jump"))
		{
			velocity.Y = jumpVelocity;
			
		}
		if (!_canDoubleJump) // TODO this has problems, the signal will be emitting every physics frame
		{
			_canDoubleJump = true;
			EmitSignal(SignalName.landed);
		}
	}


		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * speed;
			velocity.Z = direction.Z * speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion motionEvent) {
			RotateY(-motionEvent.Relative.X * lookSensitivity * ((float)Math.PI/180));
			_viewportPivot.RotateX(-motionEvent.Relative.Y * lookSensitivity * ((float)Math.PI/180));

			if (_viewportPivot.Rotation.X > Math.PI/2 || _viewportPivot.Rotation.X < -Math.PI/2) {
				_viewportPivot.Rotation = new Vector3((float)Mathf.Clamp(_viewportPivot.Rotation.X, -Math.PI/2, Math.PI/2), _viewportPivot.Rotation.Y, _viewportPivot.Rotation.Z);
			}
		}

		if (Input.IsActionJustPressed("attack") && _canAttack)
		{
			// Set _canAttack to false and start the attack cooldown timer
			_canAttack = false;
			attackCooldown.Start();
			if (attackHitbox.IsColliding())
			{
				for(int collision = 0; collision < attackHitbox.GetCollisionCount(); collision++)
				{
					var collider = attackHitbox.GetCollider(collision) as Node;
					if (collider is Hurtbox && collider.IsInGroup("Enemies"))
					{
						Hurtbox target = (Hurtbox) collider;
						target.HitRegistered(basicAttack);
					}
				}
			}
		}
    }
}
