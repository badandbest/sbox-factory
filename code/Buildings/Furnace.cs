using System;
namespace Factory.Buildings;

public class Furnace : BaseBuilding, Component.ITriggerListener
{
	public void OnTriggerEnter( Collider other )
	{
		if ( !other.Tags.Has( "ore" ) || other.Tags.Has( "melting" ) )
		{
			return;
		}

		if ( !other.Components.TryGet( out Ore ore ) )
		{
			return;
		}
		
		//Owner.Money += (int)MathF.Round( ore.Worth );
		ore.Melt();
	}
	
	public void OnTriggerExit( Collider other ) {}
}
