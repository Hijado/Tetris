using Microsoft.Xna.Framework;


class Block
{
    Color color;
    bool[,] shape = new bool[4, 4];

    public Block()
    {
        blockMaker(GameWorld.Random.Next(0, 7));
    }

    private void blockMaker(int shapeNumber)
    {
        // x and O are for readability
        bool x = false;
        bool O = true;

        switch (shapeNumber)
        {
            case 0:
                shape = new bool[,]{{x,O,x,x},
                                    {x,O,x,x},
                                    {x,O,x,x},
                                    {x,O,x,x}};
                color = Color.Blue;
                break;
            case 1:
                shape = new bool[,]{{x,O,x,x},
                                    {x,O,x,x},
                                    {x,O,O,x},
                                    {x,x,x,x}};
                color = Color.Orange;
                break;
            case 2:
                shape = new bool[,]{{x,x,O,x},
                                    {x,x,O,x},
                                    {x,O,O,x},
                                    {x,x,x,x}};
                color = Color.Brown;
                break;
            case 3:
                shape = new bool[,]{{x,O,x,x},
                                    {x,O,O,x},
                                    {x,O,x,x},
                                    {x,x,x,x}};
                color = Color.Purple;
                break;
            case 4:
                shape = new bool[,]{{x,O,x,x},
                                    {x,O,O,x},
                                    {x,x,O,x},
                                    {x,x,x,x}};
                color = Color.Green;
                break;
            case 5:
                shape = new bool[,]{{x,x,O,x},
                                    {x,O,O,x},
                                    {x,O,x,x},
                                    {x,x,x,x}};
                color = Color.Red;
                break;
            case 6:
                shape = new bool[,]{{x,x,x,x},
                                    {x,O,O,x},
                                    {x,O,O,x},
                                    {x,x,x,x}};
                color = Color.Yellow;
                break;
        }
    }

    public bool[,] Shape
    {
        get { return shape; }
    }

    public Color Color
    {
        get { return color; }
    }

    public void rotation()
    {
        bool[,] newShape = new bool[4, 4];

        for (int i = 3; i >= 0; --i)
        {
            for (int j = 0; j < 4; ++j)
            {
                newShape[j, 3 - i] = shape[i, j];
            }
        }
        shape = newShape;
    }

    public void reverseRotation()
    {
        bool[,] newShape = new bool[4, 4];

        for (int i = 3; i >= 0; --i)
        {
            for (int j = 0; j < 4; ++j)
            {
                newShape[3 - j, i] = shape[i, j];
            }
        }
        shape = newShape;
    }
}

