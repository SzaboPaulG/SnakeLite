using Silk.NET.SDL;

namespace TheAdventure;

public class Game
{
    public const int GridSize = 20;
    public const int CellSize = 40;

    private readonly Random random = new();
    private readonly List<Map> maps;

    private float moveTimer;
    private float moveInterval = 0.08f;
    private bool growNextMove;

    public Snake Snake { get; }
    public Food Food { get; }

    public Map CurrentMap { get; private set; }

    public bool GameOver { get; private set; }

    public int Score { get; private set; }

    public int HighScore { get; private set; }

    public Game()
    {
        maps = Map.CreateMaps();

        Snake = new Snake();
        Food = new Food();

        HighScore = HighScoreManager.Load();

        CurrentMap = maps[random.Next(maps.Count)];

        SpawnFood();
    }

    public void Restart()
    {
        Snake.Reset();

        CurrentMap = maps[random.Next(maps.Count)];

        Score = 0;
        GameOver = false;
        moveInterval = 0.08f;
        moveTimer = 0;
        growNextMove = false;

        SpawnFood();
    }

    public void HandleKey(KeyCode key)
    {
        if (GameOver)
        {
            if (key == KeyCode.R)
            {
                Restart();
            }

            return;
        }

        switch (key)
        {
            case KeyCode.W:
            case KeyCode.Up:
                if (Snake.Direction != Direction.Down)
                    Snake.Direction = Direction.Up;
                break;

            case KeyCode.S:
            case KeyCode.Down:
                if (Snake.Direction != Direction.Up)
                    Snake.Direction = Direction.Down;
                break;

            case KeyCode.A:
            case KeyCode.Left:
                if (Snake.Direction != Direction.Right)
                    Snake.Direction = Direction.Left;
                break;

            case KeyCode.D:
            case KeyCode.Right:
                if (Snake.Direction != Direction.Left)
                    Snake.Direction = Direction.Right;
                break;
        }
    }

    public void Update(float deltaSeconds)
    {
        if (GameOver)
            return;

        moveTimer += deltaSeconds;

        if (moveTimer < moveInterval)
            return;

        moveTimer = 0;

        Snake.Move(growNextMove);
        growNextMove = false;

        var head = Snake.Head;

        if (CurrentMap.Walls.Contains(head))
        {
            EndGame();
            return;
        }

        if (Snake.CheckSelfCollision())
        {
            EndGame();
            return;
        }

        if (head.Equals(Food.Position))
        {
            growNextMove = true;
            Score++;

            if (Score > HighScore)
            {
                HighScore = Score;
                HighScoreManager.Save(HighScore);
            }

            SpawnFood();
        }
    }

    private void EndGame()
    {
        GameOver = true;
    }

    private void SpawnFood()
    {
        while (true)
        {
            var pos = new GridPosition(
                random.Next(GridSize),
                random.Next(GridSize)
            );

            if (CurrentMap.Walls.Contains(pos))
                continue;

            if (Snake.Contains(pos))
                continue;

            Food.Position = pos;
            return;
        }
    }

    // AI-generated

    public unsafe void Render(Sdl sdl, IntPtr renderer)
    {
        var r = (Renderer*)renderer;

        sdl.SetRenderDrawColor(r, 20, 20, 20, 255);
        sdl.RenderClear(r);

        DrawWalls(sdl, r);
        DrawFood(sdl, r);
        DrawSnake(sdl, r);

        if (GameOver)
        {
            DrawGameOverOverlay(sdl, r);
        }

        sdl.RenderPresent(r);
    }

    private unsafe void DrawWalls(Sdl sdl, Renderer* renderer)
    {
        foreach (var wall in CurrentMap.Walls)
        {
            DrawCell(sdl, renderer, wall, 100, 100, 100);
        }
    }

    private unsafe void DrawFood(Sdl sdl, Renderer* renderer)
    {
        DrawCell(sdl, renderer, Food.Position, 220, 40, 40);
    }

    private unsafe void DrawSnake(Sdl sdl, Renderer* renderer)
    {
        for (int i = 0; i < Snake.Body.Count; i++)
        {
            var segment = Snake.Body[i];

            if (i == 0)
            {
                DrawCell(sdl, renderer, segment, 60, 220, 60);
            }
            else
            {
                DrawCell(sdl, renderer, segment, 30, 150, 30);
            }
        }
    }
    private unsafe void DrawCell(
        Sdl sdl,
        Renderer* renderer,
        GridPosition position,
        byte red,
        byte green,
        byte blue)
    {
        sdl.SetRenderDrawColor(renderer, red, green, blue, 255);

        int startX = position.X * CellSize;
        int startY = position.Y * CellSize;
        int endX = startX + CellSize - 1;
        int endY = startY + CellSize - 1;

        for (int y = startY + 2; y < endY - 2; y++)
        {
            sdl.RenderDrawLine(
                renderer,
                startX + 2,
                y,
                endX - 2,
                y
            );
        }
    }

    private unsafe void DrawGameOverOverlay(Sdl sdl, Renderer* renderer)
    {
        sdl.SetRenderDrawColor(renderer, 150, 0, 0, 255);

        for (int y = 320; y < 480; y++)
        {
            sdl.RenderDrawLine(renderer, 180, y, 620, y);
        }
    }

    // end AI-generated
}
