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
    Texture2D screen;

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

    bool started = false;
    bool tailMode = false;
    bool won = false;

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

    void startGame()
    {
        time = 0f;
        snake = new LinkedList<Point>();
        snake.AddLast(new Point (10, 20));
        snake.AddLast(new Point(10, 19));
        snake.AddLast(new Point(10, 18));
        snake.AddLast(new Point(10, 17));
        snake.AddLast(new Point(10, 16));
        dir = DIRECTION.UP;
        nextDir = DIRECTION.UP;
        apple = new Point(20, 20);

        moveApple();
        drawGame();
        started = true;
        tailMode = false;
        won = false;
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
        foreach (Point p in snake)
        {
            screen.SetPixel(p.X, p.Y, Color.green);
        }
        foreach (Point w in walls)
        {
            screen.SetPixel(w.X, w.Y, Color.white);
        }

        if (tailMode == true) { screen.SetPixel(snake.First.Value.X, snake.First.Value.Y, Color.red); }
        else { screen.SetPixel(apple.X, apple.Y, Color.red); }

        screen.Apply();
        GetComponent<Renderer>().material.mainTexture = screen;
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
        if (dir == DIRECTION.UP) { nextHead.Y -= 1; }
        else if (dir == DIRECTION.DOWN) { nextHead.Y += 1; }
        else if (dir == DIRECTION.LEFT) { nextHead.X += 1; }
        else if (dir == DIRECTION.RIGHT) { nextHead.X -= 1; }

        if (tailMode == true)
        {
            if (nextHead.X == snake.First.Value.X && nextHead.Y == snake.First.Value.Y) { win(); return; }
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
        if (nextHead.X == apple.X && nextHead.Y == apple.Y) { moveApple(); }
        else { snake.RemoveFirst(); }
    }

    void gameOver()
    {
        started = false;
    }

    void win()
    {
        started = false;
        won = true;
    }

    void enterTailMode()
    {
        tailMode = true;
        apple.X = 0; 
        apple.Y = 0;

        //play different sounds I guess
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.UpArrow) && dir != DIRECTION.DOWN)
        {
            nextDir = DIRECTION.UP;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && dir != DIRECTION.UP)
        {
            nextDir = DIRECTION.DOWN;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && dir != DIRECTION.RIGHT)
        {
            nextDir = DIRECTION.LEFT;
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow) && dir != DIRECTION.LEFT)
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
                drawGame();
                time = 0;
            }
        }

        else
        {
            if (Input.GetKeyDown(KeyCode.Return) && won != true)
            {
                startGame();
            }
        }

    }
}
