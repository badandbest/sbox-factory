using Sandbox.Network;
using System.Threading.Tasks;
namespace Factory;

[Title( "Game Manager" )]
[Category( "Factory" )]
[Icon( "sports_esports" )]
public sealed class GameManager : Component, Component.INetworkListener
{
	/// <summary>
	/// Create a server (if we're not joining one)
	/// </summary>
	[Property] public bool StartServer { get; set; } = true;

	/// <summary>
	/// The prefab to spawn for the player to control.
	/// </summary>
	[Property] GameObject Pawn { get; set; }

	/*
	protected override void OnStart()
	{
		if ( !IsMultiplayer )
		{
			OnActive( Connection.Local );
			return;
		}
		
		if ( !GameNetworkSystem.IsActive )
		{
			// Create a lobby if there is none.
			GameNetworkSystem.CreateLobby();
		}
	}
	*/
	
	protected override async Task OnLoad()
	{
		if ( Scene.IsEditor )
			return;

		if ( StartServer && !GameNetworkSystem.IsActive )
		{
			LoadingScreen.Title = "Creating Lobby";
			await Task.DelayRealtimeSeconds( 0.1f );
			GameNetworkSystem.CreateLobby();
		}
	}
	
	
	/// <summary>
	/// A client is fully connected to the server. This is called on the host.
	/// </summary>
	public void OnActive( Connection client )
	{
		if ( Scene.Components.TryGet( out Workspace workspace, FindMode.Disabled | FindMode.InChildren ) )
		{
			Pawn.Clone( workspace.GameObject, Vector3.Zero, Rotation.Identity, 1 );
			workspace.GameObject.NetworkSpawn( client );
		}
	}

	/*
	/// <summary>
	/// Find workspace to assign
	/// </summary>
	GameObject FindWorkspace()
	{
		//
		// If they have spawn point set then use those
		//
		if ( SpawnPoints is not null && SpawnPoints.Count > 0 )
		{
			return Random.Shared.FromList( SpawnPoints, default ).Transform.World;
		}

		//
		// If we have any SpawnPoint components in the scene, then use those
		//
		var spawnPoints = Scene.GetAllComponents<SpawnPoint>().ToArray();
		if ( spawnPoints.Length > 0 )
		{
			return Random.Shared.FromArray( spawnPoints ).Transform.World;
		}

		//
		// Failing that, spawn where we are
		//
		return Transform.World;
	}
	*/
	

	public void OnDisconnected( Connection connection )
	{
		var workspace = Scene.GetAllComponents<Workspace>().First(x => x.GameObject.Network.OwnerConnection == connection );
	}
}
