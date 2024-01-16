namespace Factory.Buildings;

[Title( "Building" )]
[Category( "Factory" )]
[Icon( "precision_manufacturing" )]
public abstract class BaseBuilding : Component
{
	//public Client Owner { get => Workspace.Owner; }
	
	public Workspace Workspace => Components.GetInParent<Workspace>();
}

