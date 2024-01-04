global using Sandbox;
using Factory.Player;
using Sandbox.Network;
using System.Collections.Generic;
using System.Linq;
namespace Factory;

[Title( "Game Manager" )]
[Category( "Factory" )]
[Icon( "sports_esports" )]
public sealed class GameManager : Component, Component.INetworkListener
{
	/// <summary>
	/// The prefab to spawn for the player to control.
	/// </summary>
	[Property] public GameObject PlayerPrefab { get; set; }
	
	[Property] public List<GameObject> SpawnPoints { get; set; }
	
	protected override void OnStart()
	{
		// There's going to be a lot of rigidbodies at once.
		Scene.PhysicsWorld.SimulationMode = PhysicsSimulationMode.Discrete;
		
		// Create a lobby if we're not connected
		if ( !GameNetworkSystem.IsActive )
		{
			GameNetworkSystem.CreateLobby();
		}
	}

	public void OnActive( Connection channel )
	{
		Log.Info( $"Player '{channel.DisplayName}' has joined the game" );
		
		var player = SceneUtility.Instantiate( PlayerPrefab, SpawnPoints.First().Transform.World );
		player.Name = $"Player - {channel.DisplayName}";
		player.BreakFromPrefab();

		// Make the client the owner
		player.Network.Spawn( channel );
		player.Components.Get<Client>().Setup( Components.GetInChildren<Workspace>() );
	}
}

