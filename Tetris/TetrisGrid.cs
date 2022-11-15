using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

/// <summary>
/// A class for representing the Tetris playing grid.
/// </summary>
class TetrisGrid
{
    Color[,] grid;

    /// Sound
    SoundEffect rowClearSound;

    /// The sprite of a single empty cell in the grid.
    Texture2D emptyCell;
    Rectangle gridSize;

    /// The position at which this TetrisGrid should be drawn.
    Vector2 position;
    Vector2 offset = new Vector2(110, 70);

    /// The number of grid elements in the x-direction.
    public int Width { get { return 10; } }
    /// The number of grid elements in the y-direction.
    public int Height { get { return 20; } }

    // declares new blocks
    Block block, blockNextBlock, blockHolding, block_Copy;

    // size of a single grid/block
    const int emptyCellSize = 30;

    // tracks if a line is cleared
    int fullBlockCounter = 0;

    int score = 0;

    // tracks how many lines are cleared
    int scoreCounter = 0;

    int level = 1;
    double levelXPNeeded = 4000;
    double nextLevelIn = 0;

    // start position without offset
    int horizontal = 90;
    int vertical = 0;

    // initial timer
    int timer = 600;

    // needed for increase of speed after lvl up
    int verticalSpeed = 100;
    int verticalSpeedAdd = 10;

    // if the block can move or not
    bool canMoveLeft = true;
    bool canRotate = true;
    bool canMoveRight = true;
    bool canMoveDown = true;

    bool end = false;
    bool firstTimeHoldingBlock = true;
    bool canHoldAgain = false;

    /// <summary>
    /// Creates a new TetrisGrid.
    /// </summary>
    /// <param name="b"></param>
    public TetrisGrid()
    {
        block = new Block();
        blockNextBlock = new Block();
        blockHolding = new Block();
        emptyCell = TetrisGame.ContentManager.Load<Texture2D>("block");
        rowClearSound = TetrisGame.ContentManager.Load<SoundEffect>("Sound");
        grid = new Color[Height, Width];
        position = Vector2.Zero;
        gridSize = new Rectangle(110, 70, 300, 600);

        // fills color grid with color white
        for (int x = 0; x < grid.GetLength(1); x++)
        {
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                grid[y, x] = Color.White;
            }
        }
    }
    public void Update(GameTime gameTime, InputHelper inputHelper)
    {
        movingBlock(inputHelper, gameTime);
    }
    public void movingBlock(InputHelper inputHelper, GameTime gameTime)
    {
        if (inputHelper.KeyPressed(Keys.A) && canRotate)
            block.rotation();
        else if (inputHelper.KeyPressed(Keys.D) && canRotate)
            block.reverseRotation();
        else if (inputHelper.KeyPressed(Keys.Left) && canMoveLeft)
        {
            horizontal -= 30;
            canMoveRight = true;
            canRotate = true;
        }
        else if (inputHelper.KeyPressed(Keys.Right) && canMoveRight)
        {
            horizontal += 30;
            canMoveLeft = true;
            canRotate = true;
        }
        else if (inputHelper.KeyPressed(Keys.Down) && canMoveDown)
        {
            vertical += 30;
        }
        else if (inputHelper.KeyPressed(Keys.C) && firstTimeHoldingBlock)
        {
            blockHolding = block;
            canMoveDown = true;
            canMoveLeft = true;
            canMoveRight = true;
            canRotate = true;
            horizontal = 90;
            vertical = 0;
            position = Vector2.Zero;
            block = blockNextBlock;
            blockNextBlock = new Block();
            firstTimeHoldingBlock = false;
        }
        else if (inputHelper.KeyPressed(Keys.C) && canHoldAgain)
        {   
            block_Copy = block;
            block = blockHolding;
            blockHolding = block_Copy;
            canMoveDown = true;
            canMoveLeft = true;
            canMoveRight = true;
            canRotate = true;
            horizontal = 90;
            vertical = 0;
            position = Vector2.Zero;
            canHoldAgain = false;

        }

        if (timer/verticalSpeed <= 1 && canMoveDown)
        {
            vertical += 30;
            verticalSpeed = 100;
        }
    }
    public void rowClearCheck()
    {
        // Checks if a row is full
        for (int j = 0; j < grid.GetLength(0); j++)
        {
            for (int i = 0; i < grid.GetLength(1); i++)
            {
                if (grid[j, i] != Color.White)
                {
                    fullBlockCounter++;
                }
            }
            if (fullBlockCounter == 10)
            {
                MoveDown(j);
                fullBlockCounter = 0;
                // Stores the amount of rows cleared
                scoreCounter++;
            }
            else
                fullBlockCounter = 0;
        }

        // Checks if a row has been removed
        if (scoreCounter > 0)
        {
            // Calculates the score, more points for tetris
            if (scoreCounter == 4)
            {
                score += 1000 * level;
            }
            else
                score += 100 * level * scoreCounter;
            // Plays rowClearSound
            scoreCounter = 0;
            rowClearSound.Play();
        }
    }

    /// <summary>
    /// Draws the grid on the screen.
    /// </summary>
    /// <param name="gameTime">An object with information about the time that has passed in the game.</param>
    /// <param name="spriteBatch">The SpriteBatch used for drawing sprites and text.</param>
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        // draws the color grid
        spriteBatch.Draw(emptyCell, gridSize, Color.Red);
        for (int x = 0; x < grid.GetLength(1); x++)
        {
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                position = offset + new Vector2(x, y) * emptyCellSize;
                // if a value of color grid is not white, draw it
                if (grid[y, x] != Color.White)
                    spriteBatch.Draw(emptyCell, position, grid[y, x]);
                else if (grid[y, x] == Color.White)
                    spriteBatch.Draw(emptyCell, position, Color.White);
            }
        }

        collision(spriteBatch, gameTime);
    }

    /// <summary>
    /// checks for collision
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name="gameTime"></param>
    public void collision(SpriteBatch spriteBatch, GameTime gameTime)
    {
        verticalSpeed += verticalSpeedAdd;
        for (int y = 0; y < block.Shape.GetLength(0); y++)
        {
            for (int x = 0; x < block.Shape.GetLength(1); x++)
            {
                // draws the next block and the block that is currently being held
                if (blockNextBlock.Shape[y,x])
                    spriteBatch.Draw(emptyCell, new Vector2(500, 200) + new Vector2(x, y) * emptyCellSize, blockNextBlock.Color);
                if (blockHolding.Shape[y,x] && firstTimeHoldingBlock == false)
                    spriteBatch.Draw(emptyCell, new Vector2(500, 500) + new Vector2(x, y) * emptyCellSize, blockHolding.Color);

                // calculates the position at which the block should be drawn
                position = offset + new Vector2(horizontal, vertical) + new Vector2(x, y) * emptyCellSize;

                // if the value of y, x is true
                if (block.Shape[y,x])
                {
                    // draw the singel block at position
                    spriteBatch.Draw(emptyCell, position, block.Color);
                    // needed for collision
                    Rectangle rectangle = new Rectangle((int)position.X, (int)position.Y, 30, 30);
                    if (rectangle.Left == gridSize.Left)
                    {
                        canMoveLeft = false;
                        canRotate = false;
                    }
                    if (rectangle.Right == gridSize.Right)
                    {
                        canMoveRight = false;
                        canRotate = false;
                    }
                    if (rectangle.Bottom == gridSize.Bottom)
                    {
                        canMoveDown = false;
                        canRotate = false;
                        canMoveRight = false;
                        canMoveLeft = false;

                        // stores the current block that has hit the bottom in the color grid
                        for (int p = 0; p < block.Shape.GetLength(0); p++)
                            for (int q = 0; q < block.Shape.GetLength(1); q++)
                            {
                                if (block.Shape[q,p])
                                    grid[q + vertical / 30, p + horizontal / 30] = block.Color;
                            }
                        rowClearCheck();
                        levelUp();
                        Clear();
                        return;
                    }
                    try
                    {
                        for (int a = 0; a < block.Shape.GetLength(0); a++)
                            for (int b = 0; b < block.Shape.GetLength(1); b++)
                                if (block.Shape[b, a])
                                {
                                    if (grid[b + vertical / 30, a + horizontal / 30] != Color.White)
                                    {
                                        canMoveDown = false;
                                        canRotate = false;
                                        canMoveRight = false;
                                        canMoveLeft = false;

                                        // stores the current block that has hit another block in the color grid
                                        for (int p = 0; p < block.Shape.GetLength(0); p++)
                                            for (int q = 0; q < block.Shape.GetLength(1); q++)
                                            {
                                                if (block.Shape[q, p])
                                                    grid[q - 1 + vertical / 30, p + horizontal / 30] = block.Color;
                                            }
                                        rowClearCheck();
                                        levelUp();
                                        Clear();
                                        return;
                                    }
                                }
                    }
                    // if the current block gets out of bound i.e higher than the grid top, end the game
                    catch (IndexOutOfRangeException)
                    {
                        end = true;
                    }
                        
                }
            }
        }
    }

    /// <summary>
    /// Replaces the row that is being cleard with the row that is above it, also adds new row on top
    /// </summary>
    /// <param name="i"> index of row that is being cleared</param>
    public void MoveDown(int i)
    {
        for (int y = i; y > 0; y--)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                grid[y, x] = grid[y - 1, x];
            }
        }
        for (int x = 0; x < grid.GetLength(1); x++)
            grid[0, x] = Color.White;
    }

    /// <summary>
    /// checks if the score has exceded the xp needed for lvl up
    /// </summary>
    public void levelUp()
    {
        if (score >= levelXPNeeded)
        {
            level++;
            levelXPNeeded += levelXPNeeded * 1.1;
            verticalSpeed += 5;
        }
        nextLevelIn = levelXPNeeded - score;
    }

    /// <summary>
    /// Clears the grid and resets the values
    /// </summary>
    public void Clear()
    {
        canMoveDown = true;
        canMoveLeft = true;
        canMoveRight = true;
        canRotate = true;
        horizontal = 90;
        vertical = 0;
        position = Vector2.Zero;
        block = blockNextBlock;
        canHoldAgain = true;
        blockNextBlock = new Block();
    }

    // get methods for drawing score etc. in GameWorld class
    public int Score { get { return score; } }

    public int Level { get { return level; } }

    public double XpNeeded { get { return nextLevelIn; } }

    public bool End { get { return end; } }
}

