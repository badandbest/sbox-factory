namespace Factory.Buildings;

public class Conveyor : Building
{
	float _speed;
	[Property,Group("Conveyor")] 
	public float Speed
	{
		get => _speed;
		set
		{
			_speed = value;
			
			if ( Components.TryGet( out ModelRenderer renderer ) )
			{
				renderer.SceneObject?.Attributes.Set( "Speed", _speed );
			}
		}
	}
	
	protected override void OnStart()
	{
		if ( !Components.TryGet( out ModelRenderer renderer ) )
		{
			return;
		}
		
		renderer.SceneObject?.Attributes.Set( "Speed", _speed );
		UpdateConveyors();
	}

	protected override void OnDestroy()
	{
		if ( !Components.TryGet( out ModelRenderer renderer ) )
		{
			return;
		}
	}

	void UpdateConveyors()
	{
		// Todo: Make this better?
		// Front
		var tr = Scene.Trace.IgnoreGameObject( GameObject ).FromTo( Transform.Position + Vector3.Up * 4, Transform.Position + Vector3.Up * 4 + Transform.Rotation.Forward * 20 ).Run();

		if ( tr.Hit && tr.GameObject.Tags.Has( "conveyor" ) && tr.GameObject.Transform.Rotation == Transform.Rotation )
		{
			Components.Get<ModelRenderer>(  ).SetBodyGroup( "head", 1 );
			tr.GameObject.Components.Get<ModelRenderer>(  ).SetBodyGroup( "tail", 1 );
		}
	
		// Back
		tr = Scene.Trace.IgnoreGameObject( GameObject ).FromTo( Transform.Position + Vector3.Up * 4, Transform.Position + Vector3.Up * 4 + Transform.Rotation.Backward * 20 ).Run();
		
		if ( tr.Hit && tr.GameObject.Tags.Has( "conveyor" ) && tr.GameObject.Transform.Rotation == Transform.Rotation )
		{
			Components.Get<ModelRenderer>(  ).SetBodyGroup( "tail", 1 );
			tr.GameObject.Components.Get<ModelRenderer>(  ).SetBodyGroup( "head", 1 );
		}
	}

	protected override void OnFixedUpdate()
	{
		// Push rigidbodies, then teleport back.
		var body = Components.GetInChildren<Collider>().KeyframeBody;


		body.Position = Transform.Position + Transform.Rotation.Backward * _speed;
		body.Move( Transform.World, Time.Delta );
	}
}
