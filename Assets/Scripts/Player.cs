
public enum Player
{
    None, Black, White
}

public static class PlayerExtensions
{
    public static Player Opponent(this Player player)
    {
        return player switch
        {
            Player.Black => Player.White,
            Player.White => Player.Black,
            _ => Player.None
        };
    }

    public static string ToString(this Player player)
    {
        return player == Player.Black ? "Black" : "White";
    }
}
