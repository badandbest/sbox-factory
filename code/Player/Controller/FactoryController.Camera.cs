using Sandbox.Citizen;
namespace Factory.Player;

public sealed partial class FactoryController
{
	/// <summary>
	/// Current facing direction.
	/// </summary>
	[Property, Category( "Camera" ), Sync( Query = true )]
	public Angles EyeAngles { get; set; }

	/// <summary>
	/// Vertical eye offset.
	/// </summary>
	[Range( 28.0f, 64.0f )]
	[Property, Category( "Camera" ), Sync( Query = true )]
	public float EyeHeight { get; set; } = 64;

	bool _firstPerson { get; set; } = true;
 	/// <summary>
	/// Are we in firstperson?
	/// </summary>
	[Property, Category( "Camera" )]
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
	
	public Ray AimRay => new( Transform.Position + new Vector3( 0f, 0f, EyeHeight ), EyeAngles.Forward );

	protected override void OnStart()
	{
		// We don't want to show up to ourselves.
		FirstPerson = !IsProxy;
	}

	protected override void OnPreRender()
	{
		Animate();
		if ( IsProxy ) return;
		
		// Mouse input.
		EyeAngles += Input.AnalogLook;
		EyeAngles = new Angles( EyeAngles.pitch.Clamp( -90, 90 ), EyeAngles.yaw, 0 );

		
		// Lerp EyeHeight so it's smooth.
		EyeHeight = EyeHeight.LerpTo( Crouched ? 28 : 64, Time.Delta * 10.0f );
		
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

	private void Animate()
	{
		var anim = Components.GetInChildren<CitizenAnimationHelper>();
		var cc = Components.Get<CharacterController>();

		anim.Target.Transform.Rotation = Rotation.FromYaw( EyeAngles.yaw );
		anim.WithLook( EyeAngles.Forward );

		anim.WithVelocity( cc.Velocity );
		anim.WithWishVelocity( WishVelocity );
		
		anim.IsGrounded = cc.IsOnGround;
		anim.DuckLevel = EyeHeight.Remap( 28, 64, 1, 0 );
	}
}
