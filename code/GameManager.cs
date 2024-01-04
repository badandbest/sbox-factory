global using Sandbox;
using Factory.Player;
using Sandbox.Network;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Factory;

[Title( "Game Manager" )]
[Category( "Factory" )]
[Icon( "sports_esports" )]
public sealed class GameManager : Component, Component.INetworkListener
{
	
	/// <summary>
	/// Is this game multiplayer?
	/// </summary>
	[Property] public bool IsMultiplayer { get; set; } = true;
	
	/// <summary>
	/// The prefab to spawn for the player to control.
	/// </summary>
	[Property] public GameObject PlayerPrefab { get; set; }
	
	[Property] public List<GameObject> SpawnPoints { get; set; }
	
	protected override void OnStart()
	{
		// There's going to be a lot of rigidbodies at once.
		Scene.PhysicsWorld.SimulationMode = PhysicsSimulationMode.Discrete;
		
		if ( !IsMultiplayer ) return;
		
		//
		// Create a lobby if we're not connected
		//
		if ( !GameNetworkSystem.IsActive )
		{
			GameNetworkSystem.CreateLobby();
		}
	}

	public void OnActive( Connection channel )
	{
		Log.Info( $"Player '{channel.DisplayName}' has joined the game" );
		
		// Spawn this object and make the client the owner
		var player = SceneUtility.Instantiate( PlayerPrefab );
		player.Name = $"Player - {channel.DisplayName}";
		player.BreakFromPrefab();
		player.Network.Spawn( channel );
		player.Transform.World = SpawnPoints.First().Transform.World;
		
		player.Components.Get<Client>().Setup( Components.GetInChildren<Workspace>() );
	}
}

