using Godot;
using System;

public partial class AudioManager : Node
{
	[Export]
	private AudioStreamPlayer[] eSFXHitPlayers;

	[Export]
	private AudioStreamPlayer[] eSFXEnemyPlayers;

	private static AudioManager mAudioManager;
	public static AudioManager Instance
		{ get { return mAudioManager; } }

	private int mHitStreamNumber = 0;
	private int mEnemyStreamNumber = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		mAudioManager = this;
	}
	
	private void PlaySound(AudioStream sound, AudioStreamPlayer[] players, ref int playerIndex)
	{
        if (players != null && players.Length > 0)
        {
            GD.Print("Audio Manager. Play Sounds");
			if (players[playerIndex].Playing) return;
            players[playerIndex].Stream = sound;
            players[playerIndex].Play();
            playerIndex = (playerIndex + 1) % players.Length;
        }
    }

	public void PlayHitSound(AudioStream sound)
	{
		PlaySound(sound, eSFXHitPlayers, ref mHitStreamNumber);

    }

	public void PlayEnemySound(AudioStream sound)
	{
		PlaySound(sound, eSFXEnemyPlayers, ref mEnemyStreamNumber);
	}
}
