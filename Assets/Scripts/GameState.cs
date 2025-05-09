

using System.Collections.Generic;

public class GameState
{
    public const int Rows = 8;
    public const int Cols = 8;

    public Player[,] Board { get; }
    public Dictionary<Player, int> DiscCount { get; }
    public Player CurrentPlayer { get; private set; }
    public bool GameOver { get; private set; }
    public Player Winner { get; private set; }
    public Dictionary<Position, List<Position>> LegalMoves { get; private set; }

    public GameState()
    {
        Board = new Player[Rows, Cols];
        Board[3, 3] = Player.White;
        Board[3, 4] = Player.Black;
        Board[4, 3] = Player.Black;
        Board[4, 4] = Player.White;
        DiscCount = new Dictionary<Player, int> { { Player.Black, 2 }, { Player.White, 2 } };
        CurrentPlayer = Player.Black;
        LegalMoves = GetLegalMoves(CurrentPlayer);
    }

    public GameState(GameState gameState)
    {
        Board = (Player[,])gameState.Board.Clone();
        DiscCount = new Dictionary<Player, int>(gameState.DiscCount);
        CurrentPlayer = gameState.CurrentPlayer;
        GameOver = gameState.GameOver;
        Winner = gameState.Winner;
        LegalMoves = new Dictionary<Position, List<Position>>(gameState.LegalMoves);
    }

    public bool MakeMove(Position pos, out MoveInfo moveInfo)
    {
        if (!LegalMoves.ContainsKey(pos))
        {
            moveInfo = null;
            return false;
        }

        Player movePlayer = CurrentPlayer;
        List<Position> outflanked = LegalMoves[pos];

        Board[pos.Row, pos.Col] = movePlayer;
        FlipDiscs(outflanked);
        UpdateDiscCounts(movePlayer, outflanked.Count);
        PassTurn();

        moveInfo = new MoveInfo
        {
            Player = movePlayer,
            Position = pos,
            Outflanked = outflanked
        };
        return true;
    }

    public IEnumerable<Position> OccupiedPositions()
    {
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Cols; col++)
            {
                if (Board[row, col] != Player.None)
                {
                    yield return new Position(row, col);
                }
            }
        }
    }

    private void UpdateDiscCounts(Player movePlayer, int outflankedCount)
    {
        DiscCount[movePlayer] += outflankedCount + 1; // +1 for the new disc
        DiscCount[movePlayer.Opponent()] -= outflankedCount;
    }

    private void FlipDiscs(List<Position> positions)
    {
        foreach (Position pos in positions)
        {
            Board[pos.Row, pos.Col] = Board[pos.Row, pos.Col].Opponent();
        }
    }

    private void ChangePlayer()
    {
        CurrentPlayer = CurrentPlayer.Opponent();
        LegalMoves = GetLegalMoves(CurrentPlayer);
    }

    private Player FindWinner()
    {
        if (DiscCount[Player.Black] > DiscCount[Player.White])
            return Player.Black;
        else if (DiscCount[Player.White] > DiscCount[Player.Black])
            return Player.White;
        else
            return Player.None; // Draw
    }

    private void PassTurn()
    {
        ChangePlayer();
        LegalMoves = GetLegalMoves(CurrentPlayer);
        if (LegalMoves.Count > 0)
        {
            return;
        }

        ChangePlayer();

        if (LegalMoves.Count == 0)
        {
            CurrentPlayer = Player.None; // No more moves available for both players
            GameOver = true;
            Winner = FindWinner();
        }
    }

    private bool IsInsideBoard(int row, int col)
    {
        return row >= 0 && row < Rows && col >= 0 && col < Cols;
    }

    private List<Position> OutflankedInDir(Position pos, Player player, int rDelta, int cDelta)
    {
        List<Position> outflanked = new List<Position>();
        int row = pos.Row + rDelta;
        int col = pos.Col + cDelta;

        while (IsInsideBoard(row, col) && Board[row, col] != Player.None)
        {
            if (Board[row, col] == player.Opponent())
            {
                outflanked.Add(new Position(row, col));
                row += rDelta;
                col += cDelta;
            }
            else if (Board[row, col] == player)
            {
                return outflanked;
            }
        }

        return new List<Position>();
    }

    private List<Position> Outflanked(Position pos, Player player)
    {
        List<Position> outflanked = new List<Position>();
        for (int rDelta = -1; rDelta <= 1; rDelta++)
        {
            for (int cDelta = -1; cDelta <= 1; cDelta++)
            {
                if (rDelta == 0 && cDelta == 0) continue;

                outflanked.AddRange(OutflankedInDir(pos, player, rDelta, cDelta));
            }
        }
        return outflanked;
    }

    private bool IsMoveLegal(Player player, Position pos, out List<Position> outflanked)
    {
        if (Board[pos.Row, pos.Col] != Player.None)
        {
            outflanked = null;
            return false;
        }

        outflanked = Outflanked(pos, player);
        return outflanked.Count > 0;
    }

    private Dictionary<Position, List<Position>> GetLegalMoves(Player player)
    {
        Dictionary<Position, List<Position>> legalMoves = new Dictionary<Position, List<Position>>();

        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Cols; col++)
            {
                Position pos = new Position(row, col);
                if (IsMoveLegal(player, pos, out List<Position> outflanked))
                {
                    legalMoves[pos] = outflanked;
                }
            }
        }

        return legalMoves;
    }
}
