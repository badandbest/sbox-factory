namespace Factory.Player;

[Group( "Factory" )]
[Title( "Factory Controller" )]
[Icon( "switch_account" )]
public sealed partial class FactoryController : Component
{
	public Ray AimRay => new( Transform.Position + new Vector3( 0f, 0f, EyeHeight ), EyeAngles.Forward );

	protected override void OnStart()
	{
		FirstPerson = !IsProxy;
	}

	protected override void OnUpdate()
	{
		if ( IsProxy ) return;
		
		CameraInput();
	}

	protected override void OnFixedUpdate()
	{
		if ( IsProxy ) return;
		
		CrouchInput();
		MovementInput();
	}
}
