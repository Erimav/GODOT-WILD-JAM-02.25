using Godot;
using System;

// Singleton Gamemanager
public class GameManager
{
    public enum GameState
    {
        Prepare,
        UseFieldItem,
        Wave
    }
    
    // STATIC PRIVATE
    private static GameManager mInstance = new GameManager();
    public static GameManager GetInstance()
    {
        if (mInstance == null)
            mInstance = new GameManager();
        return mInstance;
    }

    // PRIVATE
    private GameState mGameState;
    
    // PRIVATE METHODS
    private GameManager()
    {
        mGameState = GameState.Prepare;
    }

    // PUBLIC METHODS
    public GameState GetGameState()
    {
        return mGameState;
    }

    public void ChangeState(GameState state)
    {
        mGameState = state;
    }

    public Item ItemAtHand { get; set; }
}
