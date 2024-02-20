using Sandbox.Citizen;
namespace Factory.Player;

public sealed partial class Pawn
{
	[Property, Category( "Camera" ), Sync]
	public Angles EyeAngles { get; set; }

	[Range( 28.0f, 64.0f )]
	[Property, Category( "Camera" ), Sync]
	public float EyeHeight { get; set; } = 64;

	bool _firstPerson { get; set; } = true;
	public bool FirstPerson
	{
		get => _firstPerson;
		set 
		{ 
			_firstPerson = value; 
			foreach ( var models in Components.GetAll<ModelRenderer>( FindMode.InDescendants ) ) 
			{ 
				models.RenderType = _firstPerson ? ModelRenderer.ShadowRenderType.ShadowsOnly : ModelRenderer.ShadowRenderType.On; 
			} 
		} 
	}
	
	private void CameraInput()
	{
		// Mouse input.
		EyeAngles += Input.AnalogLook;
		EyeAngles = new Angles( EyeAngles.pitch.Clamp( -90, 90 ), EyeAngles.yaw, 0 );
	    
		// Lerp EyeHeight so it's smooth.
		EyeHeight = EyeHeight.LerpTo( Crouching ? 28 : 64, Time.Delta * 10.0f );
		
		var cam = Scene.Camera;
		cam.FieldOfView = Preferences.FieldOfView;
		cam.Transform.Rotation = EyeAngles;
		
		// Third person.
		if ( Input.Pressed( "View" ) ) FirstPerson = !FirstPerson;
		if ( !FirstPerson )
		{
			var tr = Scene.Trace.Ray( AimRay, -256 ).Radius( 6 ).Run();
			cam.Transform.Position = tr.EndPosition;
			return;
		}

		// First person.
		cam.Transform.Position = AimRay.Position;
	}
	
	protected override void OnPreRender()
	{
		var anim = Components.GetInChildren<CitizenAnimationHelper>();
		var cc = Components.Get<CharacterController>();
		
		anim.Target.Transform.Rotation = Rotation.FromYaw( EyeAngles.yaw );
		anim.WithLook( EyeAngles.Forward );
		
		anim.MoveStyle = WishVelocity.Length < 160f ? CitizenAnimationHelper.MoveStyles.Walk : CitizenAnimationHelper.MoveStyles.Run;
		anim.WithVelocity( cc.Velocity );
		anim.WithWishVelocity( WishVelocity );
		
		anim.IsGrounded = cc.IsOnGround;
		anim.DuckLevel = EyeHeight.Remap( 28, 64, 1, 0 );
	}
}
