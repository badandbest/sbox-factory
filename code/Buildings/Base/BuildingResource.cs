namespace Factory.Buildings;

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
