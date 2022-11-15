using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

/// <summary>
/// A class for representing the game world.
/// This contains the grid, the falling block, and everything else that the player can see/do.
/// </summary>
class GameWorld
{
    /// <summary>
    /// An enum for the different game states that the game can have.
    /// </summary>
    public enum GameState
    {
        Playing,
        GameOver,
        Begin
    }

    /// <summary>
    /// The random-number generator of the game.
    /// </summary>
    public static Random Random { get { return random; } }
    static Random random;

    /// <summary>
    /// The main font of the game.
    /// </summary>
    SpriteFont font;

    /// <summary>
    /// The current game state.
    /// </summary>
    GameState gameState;

    /// <summary>
    /// The main grid of the game.
    /// </summary>
    TetrisGrid grid;
    Texture2D background, beginScreen, gameOver;
    Song song;
    string state;

    public GameWorld()
    {
        random = new Random();
        gameState = GameState.Begin;

        font = TetrisGame.ContentManager.Load<SpriteFont>("Score");
        background = TetrisGame.ContentManager.Load<Texture2D>("TetrisBG");
        beginScreen = TetrisGame.ContentManager.Load<Texture2D>("Welcome");
        gameOver = TetrisGame.ContentManager.Load<Texture2D>("Game_Over");
        song = TetrisGame.ContentManager.Load<Song>("Music");
        grid = new TetrisGrid();
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Play(song);
    }

    public void HandleInput(GameTime gameTime, InputHelper inputHelper)
    {
        if (gameState == GameState.Begin && inputHelper.KeyPressed(Keys.Space))
            gameState = GameState.Playing;

        if (gameState == GameState.GameOver && inputHelper.KeyPressed(Keys.Space))
        {
            state = "reset";
        }
    }

    public void Update(GameTime gameTime, InputHelper inputHelper)
    {
        if (gameState == GameState.Playing)
        {
            grid.Update(gameTime, inputHelper);
        }
        if (grid.End)
            gameState = GameState.GameOver;
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        if (gameState == GameState.Playing)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(background, Vector2.Zero, Color.White);
            grid.Draw(gameTime, spriteBatch);
            spriteBatch.DrawString(font, "Score: " + grid.Score.ToString(), new Vector2(720, 250), Color.White);
            spriteBatch.DrawString(font, "Level: " + grid.Level.ToString(), new Vector2(720, 300), Color.White);
            spriteBatch.DrawString(font, "Xp Needed: " + (Int32)grid.XpNeeded, new Vector2(720, 350), Color.White);
            spriteBatch.DrawString(font, "Next Block", new Vector2(450, 150), Color.White);
            spriteBatch.DrawString(font, "Press C to Hold", new Vector2(450, 400), Color.White);
            spriteBatch.DrawString(font, "Holding", new Vector2(450, 450), Color.White);
            spriteBatch.End();
        }
        else if (gameState == GameState.Begin)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(beginScreen, Vector2.Zero, Color.White);
            spriteBatch.DrawString(font, "Welcome to Tetris -> Press space to PLAY!", new Vector2(150, 500), Color.White);
            spriteBatch.End();
        }
        else if (gameState == GameState.GameOver)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(gameOver, Vector2.Zero, Color.White);
            spriteBatch.DrawString(font, "Press space to PLAY AGAIN!", new Vector2(350, 500), Color.White);
            spriteBatch.End();
        }
    }

    public string State { get { return state; }}
}
