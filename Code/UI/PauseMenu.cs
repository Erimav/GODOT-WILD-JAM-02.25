using Godot;
using System;

public partial class PauseMenu : Menu
{
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("pause"))
        {
            Visible = !Visible; 
        }
    }
}
