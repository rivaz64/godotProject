#if TOOLS
using Godot;
using System;

[Tool]
public partial class myeditor : EditorPlugin
{
  Control dock;
  public override void _EnterTree()
	{
    dock = (Control)GD.Load<PackedScene>("addons/myeditor/my_dock.tscn").Instantiate();
    AddControlToDock(DockSlot.LeftUl, dock);
  }

	public override void _ExitTree()
	{
		// Clean-up of the plugin goes here.
	}
}
#endif
