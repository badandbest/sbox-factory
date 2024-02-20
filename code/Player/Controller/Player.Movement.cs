using Sandbox.Citizen;
namespace Factory.Player;

public sealed partial class Pawn
{
	[Property, Category( "Movement" ), Sync]
	Vector3 WishVelocity { get; set; }

	[Property, Category( "Movement" ), Sync]
	bool Crouching { get; set; }
	
	private float MoveSpeed
	{
		get
		{
			if ( Crouching ) return 64.0f;
			if ( Input.Down( "run" ) ) return 320.0f;
			if ( Input.Down( "walk" ) ) return 110.0f;
			
			return 190.0f;
		}
	}
	
	private void MovementInput()
	{
		var cc = Components.Get<CharacterController>();
		var direction = Input.AnalogMove * Rotation.FromYaw( EyeAngles.yaw );
		WishVelocity = direction.Normal * MoveSpeed;

		if ( cc.IsOnGround )
		{
			// Jump.
			if ( Input.Pressed( "jump" ) )
			{
				Components.GetInChildren<CitizenAnimationHelper>().TriggerJump();
				cc.Punch( Vector3.Up * 300 );
			}
			
			cc.ApplyFriction( 6.0f, 64 );
			cc.Accelerate( WishVelocity );
		}
		else
		{
			// Apply gravity.
			cc.Velocity += Scene.PhysicsWorld.Gravity * Time.Delta;

			cc.ApplyFriction( 0.2f );
			cc.Accelerate( WishVelocity.ClampLength( 50 ) );
		}

		cc.Move();
	}

	private void CrouchInput()
	{
		var cc = Components.Get<CharacterController>();
		if ( Input.Down( "Crouch" ) )
		{
			// Already crouched.
			if ( Crouching )
			{
				return;
			}

			// Crouch jump.
			if ( !cc.IsOnGround )
			{
				cc.MoveTo( Transform.Position += Vector3.Up * 28, true );
				Transform.ClearLerp();
				EyeHeight -= 28;
			}

			Crouching = true;
			cc.Height = 36;
		}
		else
		{
			// Already uncrouched.
			if ( !Crouching )
			{
				return;
			}

			// Can't uncrouch in air.
			if ( !cc.IsOnGround )
			{
				return;
			}
			
			// Check for ceiling.
			if ( cc.TraceDirection( Vector3.Up * 28 ).Hit )
			{
				return;
			}

			Crouching = false;
			cc.Height = 72;
		}
	}
}
