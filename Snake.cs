namespace TheAdventure;

public class Snake
{
    public List<GridPosition> Body { get; } = new();

    public Direction Direction { get; set; }

    public Snake()
    {
        Reset();
    }

    public void Reset()
    {
        Body.Clear();

        Body.Add(new GridPosition(10, 10));
        Body.Add(new GridPosition(9, 10));
        Body.Add(new GridPosition(8, 10));

        Direction = Direction.Right;
    }

    public GridPosition Head => Body[0];

    public void Move(bool grow)
    {
        var head = Head;

        GridPosition newHead = Direction switch
        {
            Direction.Up => new GridPosition(head.X, head.Y - 1),
            Direction.Down => new GridPosition(head.X, head.Y + 1),
            Direction.Left => new GridPosition(head.X - 1, head.Y),
            Direction.Right => new GridPosition(head.X + 1, head.Y),
            _ => head
        };

        Body.Insert(0, newHead);

        if (!grow)
        {
            Body.RemoveAt(Body.Count - 1);
        }
    }

    public bool Contains(GridPosition position)
    {
        return Body.Any(x => x.Equals(position));
    }

    public bool CheckSelfCollision()
    {
        return Body.Skip(1).Any(x => x.Equals(Head));
    }
}