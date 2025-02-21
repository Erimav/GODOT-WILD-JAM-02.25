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
    [Export]
    private Label mWaveNumber;
    [Export]
    private Label mNeedToReachFinish;

    public override void _Process(double delta)
    {
        mGameState.Text = GameManager.GetInstance().GetGameState().ToString();
        mCanSetToWave.Text = "Can Set To Wave: " + mGameController.CanSetToWave().ToString();
        mWaveNumber.Text = "Wave Number: " + mGameController.WaveNumber + " / " + mGameController.MaxWave;
        mNeedToReachFinish.Text = "Need Mob Finish Path To Win: " + (mGameController.MobFinishedPathToWin - mGameController.MobFinishedPath);
    }
}
