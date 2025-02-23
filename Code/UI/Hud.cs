using Godot;
using System;

public partial class Hud : Control
{
	[ExportCategory("External Exports")]
	[Export]
	private GameController mGameController;

	[ExportCategory("Nodes")]
	[Export]
	private Button mStartWaveButton;
    [Export]
    private Label mWaveNumber;
    [Export]
    private Label mNeedToReachFinish;

    // PUBLIC METHODS
    public override void _Ready()
    {
		mStartWaveButton.Connect("button_down", new Godot.Callable(this, "StartWave"));
        base._Ready();
    }

    public override void _Process(double delta)
    {
		StartWaveButtonProcess();
		SetTexts();
        base._Process(delta);
    }

    // PRIVATE METHODS
    private void StartWaveButtonProcess()
	{
		GameManager gameManager = GameManager.GetInstance();
		if (mGameController.CanSetToWave())
		{
			mStartWaveButton.Disabled = false;
		}
		else
		{
			mStartWaveButton.Disabled = true;
		}
	}

	private void StartWave()
	{
		GD.Print("Start Wave Button Pressed");
		mGameController.StartWave();
	}

    private void SetTexts()
    {
        mWaveNumber.Text = $"Waves remaining: {mGameController.MaxWave - mGameController.WaveNumber + 1} / {mGameController.MaxWave}";
        mNeedToReachFinish.Text = "Minions left to Win: " + (mGameController.MobFinishedPathToWin - mGameController.MobFinishedPath);
    }
}
