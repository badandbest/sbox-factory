using Factory.Tools;
namespace Factory;

[Category( "Factory" ), Icon( "switch_accounts" )]
public class Client : Component
{
	[Property]
	public int Money { get; set; }

	[Property] public BaseTool Tool { get; set; }

	protected override void OnStart()
	{
		Tool = new PlaceTool();
	}

	protected override void OnUpdate()
	{
		Tool?.Simulate( Connection.Local );
	}
}
