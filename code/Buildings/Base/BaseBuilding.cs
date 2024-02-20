using Factory.Player;
namespace Factory.Buildings;

[Title( "Building" )]
[Category( "Factory" )]
[Icon( "precision_manufacturing" )]
public abstract class BaseBuilding : Component
{
	public Workspace Workspace => Components.GetInParent<Workspace>();
	
	[Property]
	public BuildingResource Resource { get; set; }
}

