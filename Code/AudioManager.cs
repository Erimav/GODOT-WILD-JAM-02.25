using Godot;
using System;

public partial class AudioManager : Node
{
	[Export]
	private AudioStreamPlayer[] eSFXHitPlayers;

	[Export]
	private AudioStreamPlayer[] eSFXEnemyPlayers;

	[Export]
	private AudioStreamPlayer[] eSFXGeneralPlayers;

	[Export]
	private AudioStreamPlayer eMusicPlayer;

	private static AudioManager mAudioManager;
	public static AudioManager Instance
		{ get { return mAudioManager; } }

	private int mHitStreamNumber = 0;
	private int mEnemyStreamNumber = 0;
	private int mGeneralStreamNumber = 0;
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

	public void PlaySFX(AudioStream sound)
	{
		PlaySound(sound, eSFXGeneralPlayers, ref mGeneralStreamNumber);
	}

	public void PlayMusic(AudioStream music)
	{
		if (eMusicPlayer.Playing) eMusicPlayer.Stop();
		eMusicPlayer.Stream = music;
		eMusicPlayer.Play();
	}
}
