namespace Factory.Buildings;

[GameResource("Building Definition", "building", "A placable building.", Icon = "precision_manufacturing", IconBgColor = "#fdea60", IconFgColor = "black")]
public sealed class BuildingResource : GameResource
{
	public string Name { get; set; }
	
	public string Description { get; set; }

	public int Cost { get; set; }
	
	public GameObject Prefab { get; set; }
}
