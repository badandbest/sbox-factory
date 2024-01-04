using System.Linq;
namespace Factory.Buildings;

public sealed class Dropper : Building
{
	[Property, Group( "Ore" )] 
	public Vector3 DropPoint { get; set; }

	[Property, Group( "Ore" )]
	public GameObject Ore { get; set; }
	
	[Property, Group( "Timings" )] 
	public float Interval { get; set; }

	protected override void OnFixedUpdate()
	{
		// Using tick rate so that all droppers fire at the same time.
		if ( Time.Tick % (Scene.FixedUpdateFrequency * Interval) == 0 ) 
		{
			Drop();
		}
	}

	public void Drop()
	{
		var ore = SceneUtility.Instantiate( Ore, Transform.World.PointToWorld( DropPoint ) );
		ore.SetParent( Scene.Children.First() ); // Get it the fuck out of the inspector god damn!!!
	}
}
