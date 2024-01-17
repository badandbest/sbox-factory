using Sandbox.Citizen;
namespace Factory.Player;

public sealed partial class FactoryController
{
	[Property, Category( "Movement" ), Sync( Query = true )]
	Vector3 WishVelocity { get; set; }

	[Sync] bool _crouched { get; set; }
	/// <summary>
	/// Are we crouching?
	/// </summary>
	[Property, Category( "Movement" )]
	bool Crouched
	{
		get => _crouched;
		set
		{
			if ( _crouched == value ) return;
			_crouched = value;

			Components.Get<CharacterController>().Height = _crouched ? 36 : 72;
		}
	}
	
	float MoveSpeed
	{
		get
		{
			if ( Crouched ) return 64.0f;
			if ( Input.Down( "run" ) ) return 320.0f;
			if ( Input.Down( "walk" ) ) return 110.0f;
			
			return 190.0f;
		}
	}

	protected override void OnFixedUpdate()
	{
		if ( IsProxy ) return;
		var cc = Components.Get<CharacterController>();

		var direction = Input.AnalogMove.Normal * Rotation.FromYaw( EyeAngles.yaw );
		WishVelocity = direction * MoveSpeed;

		if ( cc.IsOnGround )
		{
			// Jump.
			if ( Input.Pressed( "jump" ) )
			{
				Components.GetInChildren<CitizenAnimationHelper>().TriggerJump();
				cc.Punch( Vector3.Up * 300 );
			}
		}
		// Apply gravity.
		else cc.Velocity += Scene.PhysicsWorld.Gravity * Time.Delta;
		
	
		if ( Input.Down( "Crouch" ) )
		{
			// Crouch jump.
			if ( !cc.IsOnGround && !Crouched )
			{
				cc.MoveTo( Transform.Position += Vector3.Up * 28, true );
				Transform.ClearLerp();
				EyeHeight -= 28;
			}
			Crouched = true;
		}
		// Check for ceiling.
		else if ( cc.IsOnGround && !cc.TraceDirection( Vector3.Up * 28 ).Hit ) Crouched = false;
		
		
		cc.ApplyFriction( cc.IsOnGround ? 6.0f : 0.2f );
		cc.Accelerate( cc.IsOnGround ? WishVelocity : WishVelocity.ClampLength( 50 ) );

		cc.Move();
	}
}
