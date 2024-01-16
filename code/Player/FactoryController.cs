using System.Linq;
namespace Factory.Player;

[Group( "Factory" )]
[Title( "Factory Controller" )]
[Icon( "switch_account" )]
public sealed partial class FactoryController : Component
{
	public Workspace Workspace => Scene.Components.GetAll<Workspace>( FindMode.InDescendants ).First( x => x.Owner == Network.OwnerId );
	
	protected override void OnEnabled()
	{
		Workspace.GameObject.Enabled = true;
	}

	protected override void OnDisabled()
	{
		Workspace.GameObject.Enabled = false;
	}
}
