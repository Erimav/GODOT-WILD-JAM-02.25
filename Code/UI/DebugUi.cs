using Godot;
using System;

public partial class DebugUi : Control
{
    [ExportCategory("External Dependency")]
    [Export]
    private GameController mGameController;

    [ExportCategory("ExportNodes")]
    [Export]
    private Label mGameState;
    [Export]
    private Label mCanSetToWave;

    public override void _Process(double delta)
    {
        mGameState.Text = GameManager.GetInstance().GetGameState().ToString();
        mCanSetToWave.Text = "Can Set To Wave: " + mGameController.CanSetToWave().ToString();
    }
}
