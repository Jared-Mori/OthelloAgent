public class Position
{
    public int Row { get; set; }
    public int Col { get; set; }

    public Position(int row, int col)
    {
        Row = row;
        Col = col;
    }

    public override bool Equals(object obj)
    {
        if (obj is Position other)
        {
            return Row == other.Row && Col == other.Col;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return 8 * Row + Col;
    }
}
