
using Factory.Buildings;
using Factory.Player;
using static Sandbox.SceneUtility;
namespace Factory;

[Title("Workspace")]
[Category("Factory")]
[Icon("grid_on")]
public sealed class Workspace : Component, Component.INetworkListener
{
	[Property]
	public Client Owner { get; set; }
	
	[Property]
	public BBox Bounds { get; set; }

	protected override void DrawGizmos()
	{
		Gizmo.Draw.Color = Gizmo.Colors.Green;
		Gizmo.Draw.LineBBox( Bounds );
	}
	
	/// <summary>
	/// Snap to workspace grid.
	/// </summary>
	public Transform SnapToGrid( Vector3 hitPosition, float yaw = 0 )
	{
		var position = Transform.World.PointToLocal( hitPosition ).SnapToGrid( 16 );
		var rotation = Rotation.FromYaw( yaw );

		return new Transform( position, rotation );
	}

	/// <summary>
	/// Construct a building.
	/// </summary>
	public void Place( BuildingResource building, Transform transform )
	{
		Owner.Money -= building.Cost;
		Instantiate( GetPrefabScene( building.Prefab ), transform ).SetParent( GameObject );
	}
}
