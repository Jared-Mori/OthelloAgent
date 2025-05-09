using UnityEngine;

public class AIAgent : MonoBehaviour
{

    // Basic AI agent using simple evaluation function
    public Position FindBestMove(GameState gameState, Player currentPlayer)
    {
        Position bestMove = null;
        int best_score = -1;

        foreach (Position move in gameState.LegalMoves.Keys)
        {
            GameState simulatedGameState = new GameState(gameState);
            MoveInfo moveInfo = null;
            simulatedGameState.MakeMove(move, out moveInfo);
            int score = moveInfo.Outflanked.Count;
            
            if (score > best_score)
            {
                best_score = score;
                bestMove = move;
            }
        }

        return bestMove;
    }

    // Minimax algorithm with depth limit
    public Position EvaluateMoveDepth(GameState gameState, Player currentPlayer, int depth)
    {
        Position bestMove = null;
        int bestScore = int.MinValue;

        foreach (Position move in gameState.LegalMoves.Keys)
        {
            GameState simulatedGameState = new GameState(gameState);
            MoveInfo moveInfo = null;
            simulatedGameState.MakeMove(move, out moveInfo);

            // Recursively evaluate the move
            int score = Minimax(simulatedGameState, currentPlayer, depth - 1, false);

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }

        return bestMove;
    }

    private int Minimax(GameState gameState, Player currentPlayer, int depth, bool isMaximizing)
    {
        // Base case: if depth is 0 or the game is over
        if (depth == 0 || gameState.GameOver)
        {
            return EvaluateGameState(gameState, currentPlayer);
        }

        int bestScore = isMaximizing ? int.MinValue : int.MaxValue;

        foreach (Position move in gameState.LegalMoves.Keys)
        {
            GameState simulatedGameState = new GameState(gameState);
            MoveInfo moveInfo = null;
            simulatedGameState.MakeMove(move, out moveInfo);

            int score = Minimax(simulatedGameState, currentPlayer, depth - 1, !isMaximizing);

            if (isMaximizing)
            {
                bestScore = Mathf.Max(bestScore, score);
            }
            else
            {
                bestScore = Mathf.Min(bestScore, score);
            }
        }

        return bestScore;
    }

    private int EvaluateGameState(GameState gameState, Player currentPlayer)
    {
        // Simple evaluation: count the number of discs for the current player
        int score = 0;

        foreach (Position pos in gameState.OccupiedPositions())
        {
            if (gameState.Board[pos.Row, pos.Col] == currentPlayer)
            {
                score++;
            }
            else
            {
                score--;
            }
        }

        return score;
    }
}


