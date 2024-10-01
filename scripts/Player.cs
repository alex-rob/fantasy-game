using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export] public float Speed = 5.0f;
	[Export] public float JumpVelocity = 4.5f;
	[Export] public float LookSensitivity = 0.3f;
	[Export] public Camera3D Viewport;

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion motionEvent) {
			RotateY(-motionEvent.Relative.X * LookSensitivity * ((float)Math.PI/180));
			Viewport.RotateX(-motionEvent.Relative.Y * LookSensitivity * ((float)Math.PI/180));

			if (Viewport.Rotation.X > Math.PI/2 || Viewport.Rotation.X < -Math.PI/2) {
				Viewport.Rotation = new Vector3((float)Mathf.Clamp(Viewport.Rotation.X, -Math.PI/2, Math.PI/2), Viewport.Rotation.Y, Viewport.Rotation.Z);
			}
		}
    }
}
