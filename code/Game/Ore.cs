namespace Factory;

[Title("Ore")]
[Category("Factory")]
[Icon("attach_money")]
public class Ore : Component
{
	[Property]
	public float Worth { get; set; }
	
	public async void Melt()
	{
		Tags.Add( "melting" );
		var renderer = Components.Get<ModelRenderer>();
		
		var rb = Components.Get<Rigidbody>();
		rb.AngularDamping = 50;
		rb.LinearDamping = 10;

		TimeUntil time = 5;
		while ( time.Fraction < 1 )
		{
			// Shrink and fade
			Transform.Scale = 1 - time.Fraction;
			
			renderer.Tint = renderer.Tint.WithAlpha( 1 - time.Fraction );

			await Task.Frame();
		}
		GameObject.Destroy();
	}
}
