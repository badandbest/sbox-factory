using System;
namespace Factory;

[Title( "Workspace" )]
[Category( "Factory" )]
[Icon( "grid_on" )]
public class Workspace : Component
{
	[Property, Sync]
	public Guid Owner { get; set; }
}
