using Factory.Buildings;
namespace Factory.Player;

[Title( "Client" )]
[Category( "Factory" )]
[Icon( "accessible_forward" )]
public sealed partial class Client : Component
{
	[Property] public int Money { get; set; } = 10;
	
	[Property] public Workspace Workspace { get; set; }

	BuildingResource _building;
	[Property] public BuildingResource Building
	{
		get => _building;
		set
		{
			if ( _building == value )
			{
				return;
			}
			
			_building = value;

			if ( _building != null )
			{
				Build();
			}
		}
	}

	/// <summary>
	/// Constructs a ray using the camera's GameObject
	/// </summary>
	public Ray AimRay
	{
		get
		{
			var camera = Components.GetInChildren<CameraComponent>();
			return new Ray( camera.Transform.Position, camera.Transform.Rotation.Forward );
		}
	}

	/// <summary>
	/// Set up the player.
	/// </summary>
	public void Setup( Workspace workspace )
	{
		Workspace = workspace;
		workspace.Network.TakeOwnership();
		workspace.Owner = this;
		
		if ( IsProxy )
		{
			return;
		}
		
		// Enable camera
		Components.GetInChildren<CameraComponent>( true ).Enabled = true;
		
		// Hide body
		Components.GetInChildren<ModelRenderer>( true ).RenderType = ModelRenderer.ShadowRenderType.ShadowsOnly;
	}
	
	protected override void OnUpdate()
	{
		// show me da money!!!
		Gizmo.Draw.ScreenText( $"${Money}", Screen.Width / 2, "Poppins", 24 );
		
		if ( Input.Pressed( "Menu" ) )
		{
			Building = null;
			return;
		}

		if ( Input.Pressed( "Slot1" ) )
		{
			Building = ResourceLibrary.Get<BuildingResource>( "data/conveyor.building" );
			return;
		}
		
		if ( Input.Pressed( "Slot2" ) )
		{
			Building = ResourceLibrary.Get<BuildingResource>( "data/dropper.building" );
			return;
		}
		
		if ( Input.Pressed( "Slot3" ) )
		{
			Building = ResourceLibrary.Get<BuildingResource>( "data/furnace.building" );
			return;
		}
	}
	
	/// <summary>
	/// Enter build mode.
	/// </summary>
	public async void Build()
	{
		// Setup game object
		var preview = new GameObject( true, "Placement Preview" ).Components.Create<ModelRenderer>();
		preview.RenderType = ModelRenderer.ShadowRenderType.Off;
		preview.Model = Building.Model;
		preview.MaterialGroup = "blueprint";
		preview.GameObject.SetParent( Workspace.GameObject );

		// Setup trace
		var trace = Scene.Trace.WithTag( "solid" ).WithoutTags( "ore", "ignore" );
		var rotation = 0;
		
		var currentBuilding = _building;
		while ( _building == currentBuilding )
		{
			//
			// Position
			//
			
			if ( Input.Pressed( "rotate" ) ) rotation += 90;
			
			if ( trace.Ray( AimRay, 1024 ).Run() is { Hit: true } tr )
			{
				preview.Transform.Local = Workspace.SnapToGrid( tr.HitPosition, rotation ).WithScale( 1f );
			}

			//
			// Placement
			//
			
			if ( CanPlace( preview ) && Input.Down( "attack1" ) )
			{
				Workspace.Place( Building, preview.Transform.World );
			}

			await Task.Frame();
		}

		preview.GameObject.Destroy();
	}

	/// <summary>
	/// Checks if we can place the building.
	/// </summary>
	bool CanPlace( ModelRenderer renderer )
	{
		renderer.Tint = Color.Red;

		// Can we afford it?
		if ( Money < Building.Cost )
		{
			Gizmo.Draw.Text( "Insufficient funds.", renderer.Transform.World, "Poppins");
			return false;
		}
		
		var bounds = Scene.Trace
			.FromTo( renderer.Transform.Position, renderer.Transform.Position )
			.UseHitboxes()
			.WithoutTags( "ore", "ignore" );
		
		// Is there something in the way?
		foreach ( var shape in renderer.Model.HitboxSet.All )
		{
			if ( shape.Shape is BBox bBox && 
				bounds.Size( bBox.Rotate( renderer.Transform.Rotation ) ).Run().Hit )
			{
				Gizmo.Draw.Text( "Invalid build location.", renderer.Transform.World, "Poppins" );
				return false;
			}
		}

		renderer.Tint = Color.White;
		return true;
	}


}

