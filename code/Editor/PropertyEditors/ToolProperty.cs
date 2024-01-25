using Editor;
using Factory.Tools;
using Sandbox;

namespace Factory;

[CustomEditor( typeof( BaseTool ) )]
public class ToolProperty : ControlWidget
{
	public ToolProperty( SerializedProperty property ) : base( property ) {}
	
	protected override void OnPaint()
	{
		base.OnPaint();
		
		var rect = LocalRect.Shrink( 6, 0 );
		var value = SerializedProperty.GetValue<BaseTool>();
		var type = EditorTypeLibrary.GetType( SerializedProperty.PropertyType );

		if ( value is null )
		{
			Paint.SetPen( Theme.ControlText.WithAlpha( 0.3f ) );
			Paint.DrawIcon( rect, type?.Icon, 14, TextFlag.LeftCenter );
			rect.Left += 22;
			Paint.DrawText( rect, $"No Tool", TextFlag.LeftCenter );
			Cursor = CursorShape.None;
		}
		else
		{
			Paint.SetPen( Theme.Green );
			Paint.DrawIcon( rect, type?.Icon, 14, TextFlag.LeftCenter );
			rect.Left += 22;
			Paint.DrawText( rect, $"{value.ToString().ToTitleCase()}", TextFlag.LeftCenter );
			Cursor = CursorShape.Finger;
		}
	}
}
