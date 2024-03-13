using Factory.Buildings;
namespace Factory;

[Title( "Workspace" )]
[Category( "Factory" )]
[Icon( "grid_on" )]
public class Workspace : Component, Component.INetworkSpawn
{
	[Property] public int Balance { get; set; }

	[Property] public Dictionary<BuildingResource, int> Inventory { get; set; } = new();

	[Property] public IEnumerable<BaseBuilding> Buildings => Components.GetAll<BaseBuilding>();
	public void OnNetworkSpawn( Connection client )
	{
	}
}
