using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Point
{
    public int X { get; set; }
    public int Y { get; set; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
}

enum DIRECTION {UP, RIGHT, DOWN, LEFT }

public class Snake : MonoBehaviour
{
    [SerializeField]
    Texture2D screen;
    public TextAsset gameOverScreen;

    [SerializeField]
    Texture2D eyeOpenSheet;
    int eyeframe = 0;

    public int screenWidth = 32;
    public int screenHeight = 32;

    public int gridSize = 30;

    DIRECTION dir;
    DIRECTION nextDir;

    LinkedList<Point> snake;
    List<Point> walls;

    Point apple;

    float time;
    [SerializeField]
    float gameTime = 0.5f;

    public bool started = false;
    bool gameLost = false;
    bool tailMode = false;
    bool won = false;
    bool needToMove = true;
    bool needToMovePupil = true;
    public bool gameComplete = false;

    List<Point> eyeList;
    List<Point> finalSnake;

    [SerializeField]
    AudioSource collectSound;
    [SerializeField]
    AudioSource hum;
    [SerializeField]
    AudioSource startup;
    [SerializeField]
    AudioSource death;


    // Start is called before the first frame update
    void Awake()
    {
        screen = new Texture2D(screenWidth, screenHeight);
        drawBlackScreen();

        screen.filterMode = FilterMode.Point;
        screen.Apply();
        GetComponent<Renderer>().material.mainTexture = screen;
        walls = new List<Point>();
        createWalls();

        eyeList = new List<Point>();

        eyeList.Add(new Point(17, 18));
        eyeList.Add(new Point(16, 18));
        eyeList.Add(new Point(15, 18));
        eyeList.Add(new Point(14, 18));

        eyeList.Add(new Point(14, 17));
        eyeList.Add(new Point(14, 16));
        eyeList.Add(new Point(14, 15));

        eyeList.Add(new Point(14, 14));
        eyeList.Add(new Point(15, 14));
        eyeList.Add(new Point(16, 14));
        eyeList.Add(new Point(17, 14));
        eyeList.Add(new Point(18, 14));

        eyeList.Add(new Point(18, 15));
        eyeList.Add(new Point(18, 16));
        eyeList.Add(new Point(18, 17));
        eyeList.Add(new Point(18, 18));
    }

    void drawBlackScreen()
    {
        for (int x = 0; x < screenWidth; x++)
        {
            for (int y = 0; y < screenHeight; y++)
            {
                screen.SetPixel(x, y, Color.black);
            }
        }
    }

    public void startGame()
    {
        GetComponent<Renderer>().material.mainTexture = screen;
        time = 0f;
        snake = new LinkedList<Point>();
        snake.AddLast(new Point (10, 16));
        snake.AddLast(new Point(10, 17));
        snake.AddLast(new Point(10, 18));
        snake.AddLast(new Point(10, 19));
        snake.AddLast(new Point(10, 20));
        dir = DIRECTION.UP;
        nextDir = DIRECTION.UP;
        apple = new Point(20, 20);

        moveApple();
        drawGame();
        started = true;
        gameLost = false;
        tailMode = false;
        won = false;
        startup.Play();
    }

    void createWalls()
    {
        int margin = (screenHeight - gridSize)/2;
        for (int x = 0; x < screenWidth; x++)
        {
            for (int y = 0; y < screenHeight; y++)
            {
                if ((y >= margin && y < screenHeight - margin) && (x == margin || x == screenWidth - margin - 1)) { walls.Add(new Point (x, y)); }
                else if ((x >= margin && x < screenWidth - margin) && (y == margin || y == screenHeight - margin - 1)) { walls.Add(new Point(x, y)); }
            }
        }
    }

    void drawGame()
    {
        drawBlackScreen();
        if (!won)
        {
            foreach (Point p in snake)
            {
                screen.SetPixel(p.X, p.Y, Color.green);
            }
        }
        else
        {
            foreach (Point p in finalSnake)
            {
                screen.SetPixel(p.X, p.Y, Color.green);
            }
            if (tailMode == true) { screen.SetPixel(finalSnake[15].X, finalSnake[15].Y, Color.red); }
        }
        foreach (Point w in walls)
        {
            screen.SetPixel(w.X, w.Y, Color.white);
        }

        if (tailMode == true) { if (!won) { screen.SetPixel(snake.First.Value.X, snake.First.Value.Y, Color.red); } }
        else { screen.SetPixel(apple.X, apple.Y, Color.red); }

        screen.Apply();
    }

    void moveApple()
    {
        if (snake.Count >= 16) { enterTailMode();  return;}
        int margin = (screenHeight - gridSize)/2;
        apple.X = Random.Range(margin + 1, screenWidth - margin - 1);
        apple.Y = Random.Range(margin + 1, screenHeight - margin - 1);

        foreach (Point p in snake)
        {
            if (apple.X == p.X && apple.Y == p.Y) { moveApple(); }
        }
    }

    private void moveSnake()
    {
        Point nextHead = snake.Last.Value;
        if (dir == DIRECTION.UP) { nextHead.Y += 1; }
        else if (dir == DIRECTION.DOWN) { nextHead.Y -= 1; }
        else if (dir == DIRECTION.LEFT) { nextHead.X -= 1; }
        else if (dir == DIRECTION.RIGHT) { nextHead.X += 1; }

        if (tailMode == true)
        {
            if (nextHead.X == snake.First.Value.X && nextHead.Y == snake.First.Value.Y) { collectSound.Play(); win(); return; }
        }

        foreach(Point body in snake)
        {
            if (nextHead.X == body.X && nextHead.Y == body.Y) { gameOver(); return; }
        }
        foreach (Point wall in walls)
        {
            if (nextHead.X == wall.X && nextHead.Y == wall.Y) { gameOver(); return; }
        }

        snake.AddLast(nextHead);
        if (nextHead.X == apple.X && nextHead.Y == apple.Y) { moveApple(); collectSound.Play(); }
        else { snake.RemoveFirst(); }

        drawGame();
    }

    void gameOver()
    {
        started = false;
        gameLost = true;
        screen.LoadImage(gameOverScreen.bytes);
        screen.Apply();

        hum.Stop();
        death.Play();
    }

    void win()
    {
        started = false;
        won = true;

        finalSnake = new List<Point>();
        foreach (Point part in snake)
        {
            finalSnake.Add(part);
        }
        gameTime = 0.5f;
    }

    void enterTailMode()
    {
        tailMode = true;
        apple.X = 0; 
        apple.Y = 0;

        hum.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!won)
        {
            if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && dir != DIRECTION.DOWN)
            {
                nextDir = DIRECTION.UP;
            }
            else if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && dir != DIRECTION.UP)
            {
                nextDir = DIRECTION.DOWN;
            }
            else if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) && dir != DIRECTION.RIGHT)
            {
                nextDir = DIRECTION.LEFT;
            }
            else if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) && dir != DIRECTION.LEFT)
            {
                nextDir = DIRECTION.RIGHT;
            }

            if (started)
            {
                time += Time.deltaTime;
                if (time > gameTime)
                {
                    dir = nextDir;
                    moveSnake();
                    time = 0;
                }
            }

            else if(gameLost)
            {
                if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.F))
                {
                    startGame();
                }
            }
        }
        else
        {
            time += Time.deltaTime;
            if (time > gameTime)
            {
                if (needToMove) { moveTowardsEye(); }
                else if (needToMovePupil) { movePupil(); }
                else if (eyeframe < 30) { playEyeAnimation(); }
                else if(!gameComplete) { gameComplete = true; }
                time = 0;
            }
        }

    }


    //end game vid logic
    void moveTowardsEye()
    {
        needToMove = false;

        for (int i = 0; i < 16; i++)
        {
            Point dest = eyeList[i];
            Point curr = finalSnake[i];

            if(curr.X < dest.X){ curr.X += 1; needToMove = true; }
            else if (curr.X > dest.X) { curr.X -= 1; needToMove = true; }
            if (curr.Y < dest.Y) { curr.Y += 1; needToMove = true; }
            else if (curr.Y > dest.Y) { curr.Y -= 1; needToMove = true; }

            finalSnake[i] = curr;
        }

        if (!needToMove) { apple.X = finalSnake[15].X; apple.Y = finalSnake[15].Y;gameTime = 0.75f; tailMode = false; }
        drawGame();
    }

    void movePupil()
    {
        needToMovePupil = false;

        if (apple.X < 16) { apple.X += 1; needToMovePupil = true; }
        else if (apple.X > 16) { apple.X -= 1; needToMovePupil = true; }
        else if (apple.Y < 16) { apple.Y += 1; needToMovePupil = true; }
        else if (apple.Y > 16) { apple.Y -= 1; needToMovePupil = true; }

        

        drawGame();

        if (needToMovePupil == false) { screen = new Texture2D(64, 64); gameTime = 0.5f; screen.filterMode = FilterMode.Point; }
    }

    void playEyeAnimation()
    {
        Color[] spriteSheet = eyeOpenSheet.GetPixels();
        Color[] frame = new Color[64 * 64];


        for(int x = 0; x < 64; x++)
        {
            for(int y = 0; y < 64; y++)
            {
                frame[x + y * 64] = spriteSheet[x + eyeframe * 64 + y * 64 * 30];
            }
        }

        screen.SetPixels(frame);
        screen.Apply();
        GetComponent<Renderer>().material.mainTexture = screen;


        eyeframe++;
    }

}
