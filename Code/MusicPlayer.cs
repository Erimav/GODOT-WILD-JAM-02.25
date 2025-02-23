using Godot;
using System;

//Add this node to the scene and it will play music on Ready
public partial class MusicPlayer : Node
{
	[Export]
	private AudioStream eMusicToPlay;

    public override void _Ready()
    {
        AudioManager.Instance.PlayMusic(eMusicToPlay);
    }
}
