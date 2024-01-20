global using Sandbox;
using Sandbox.Diagnostics;
using Sandbox.Network;
namespace Factory;

[Title( "Game Manager" )]
[Category( "Factory" )]
[Icon( "sports_esports" )]
public sealed class GameManager : Component, Component.INetworkListener
{
	/// <summary>
	/// Is this lobby multiplayer?
	/// </summary>
	[Property]
	bool IsMultiplayer { get; set; }

	/// <summary>
	/// The prefab to spawn for the player to control.
	/// /// </summary>
	[Property]
	GameObject PlayerPrefab { get; set; }

	protected override void OnAwake()
	{
		if ( IsMultiplayer )
		{
			// Create a lobby if we're not connected
			if ( !GameNetworkSystem.IsActive ) GameNetworkSystem.CreateLobby();
		}
		else OnActive( Connection.Local );
	}
	
	public void OnActive( Connection channel )
	{
		// Get an available workspace.
		var workspace = Components.Get<Workspace>( FindMode.DisabledInSelfAndChildren );
		Assert.NotNull( workspace, "No available workspaces." );
		workspace.Owner = channel.Id;
		
		// Spawn this object and make the client the owner
		var player = PlayerPrefab.Clone( global::Transform.Zero, name: "Player" );
		player.NetworkSpawn( channel );
	}
}
