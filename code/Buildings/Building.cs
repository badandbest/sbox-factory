using Factory.Player;
namespace Factory.Buildings;

[Title( "Building" )]
[Category( "Factory" )]
[Icon( "precision_manufacturing" )]
public abstract class Building : Component
{
	public Client Owner { get => Workspace.Owner; }
	
	public Workspace Workspace => Components.GetInParent<Workspace>();


}

[GameResource("Building Definition", "building", "A placable building.", Icon = "precision_manufacturing", IconBgColor = "#fdea60", IconFgColor = "black")]
public sealed class BuildingResource : GameResource
{
	public string Name { get; set; }
	
	public string Description { get; set; }
	
	[ResourceType("png")]
	public string Icon { get; set; }

	public int Cost { get; set; }
	
	public Model Model { get; set; }

	public PrefabFile Prefab { get; set; }
}

