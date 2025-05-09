using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum GameType
{
    PlayerVsPlayer,
    PlayerVsWeakAI,
    PlayerVsStrongAI,
    WeakAIvsWeakAI,
    WeakAIvsStrongAI,
    StrongAIvsStrongAI,
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Disc discBlackUp;
    [SerializeField] private Disc discWhiteUp;
    [SerializeField] private GameObject highlightPrefab;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameType gameType = GameType.PlayerVsPlayer;
    [SerializeField] private GameObject gameModeSelectionPanel;
    private int aiDepth = 3;

    private Dictionary<Player, Disc> discPrefabs = new Dictionary<Player, Disc>();
    private GameState gameState = new GameState();
    private Disc[,] discs = new Disc[8,8];
    private InputAction escapeAction, clickAction;
    private List<GameObject> highlights = new List<GameObject>();
    private bool gameModeSelected = false;
    private bool isTurnInProgress = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        discPrefabs[Player.Black] = discBlackUp;
        discPrefabs[Player.White] = discWhiteUp;

        AddStartDiscs();
        ShowLegalMoves();
        uiManager.SetPlayerText(gameState.CurrentPlayer);

        escapeAction = InputSystem.actions.FindAction("Escape");
        clickAction = InputSystem.actions.FindAction("Attack");
    }

    public void SetGameType(int type)
    {
        gameType = (GameType)type;
    }

    public void OnsliderValueChanged(float value)
    {
        aiDepth = (int)value;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!gameModeSelected || isTurnInProgress)
        {
            return;
        }

        switch (gameType)
        {
            case GameType.PlayerVsPlayer:
                PlayerTurn();
                break;
            case GameType.PlayerVsWeakAI:
                if (gameState.CurrentPlayer == Player.White)
                {
                    isTurnInProgress = true;
                    StartCoroutine(WeakAITurnCoroutine());
                }
                else
                {
                    PlayerTurn();
                }
                break;
            case GameType.PlayerVsStrongAI:
                if (gameState.CurrentPlayer == Player.White)
                {
                    isTurnInProgress = true;
                    StartCoroutine(StrongAITurnCoroutine());
                }
                else
                {
                    PlayerTurn();
                }
                break;
            case GameType.WeakAIvsWeakAI:
                isTurnInProgress = true;
                StartCoroutine(WeakAITurnCoroutine());
                break;
            case GameType.WeakAIvsStrongAI:
                isTurnInProgress = true;
                if (gameState.CurrentPlayer == Player.White)
                {
                    StartCoroutine(StrongAITurnCoroutine());
                }
                else
                {
                    StartCoroutine(WeakAITurnCoroutine());
                }
                break;
            case GameType.StrongAIvsStrongAI:
                isTurnInProgress = true;
                StartCoroutine(StrongAITurnCoroutine());
                break;
        }
    }

    public void SelectGameMode()
    {
        gameModeSelected = true;
        gameModeSelectionPanel.SetActive(false);
    }

    private void PlayerTurn()
    {
        // Handle Escape key using the new Input System
        if (escapeAction.IsPressed())
        {
            Application.Quit();
        }

        // Handle mouse click using the new Input System
        if (clickAction.IsPressed())
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            
            {
                Vector3 impact = hitInfo.point;
                Position boardPos = SceneToBoardPosition(impact);
                Debug.Log($"Clicked on board position: {boardPos}");
                OnBoardClicked(boardPos);
            }
        }
    }

    private void WeakAITurn()
    {
        StartCoroutine(WeakAITurnCoroutine());
    }

    private System.Collections.IEnumerator WeakAITurnCoroutine()
    {
        AIAgent aiAgent = new AIAgent();
        Position bestMove = aiAgent.FindBestMove(gameState, gameState.CurrentPlayer);
        if (bestMove != null)
        {
            yield return OnBoardClickedCoroutine(bestMove);
        }
    }

    private void StrongAITurn()
    {
        StartCoroutine(StrongAITurnCoroutine());
    }

    private System.Collections.IEnumerator StrongAITurnCoroutine()
    {
        AIAgent aiAgent = new AIAgent();
        Position bestMove = aiAgent.EvaluateMoveDepth(gameState, gameState.CurrentPlayer, aiDepth);
        if (bestMove != null)
        {
            yield return OnBoardClickedCoroutine(bestMove);
        }
    }

    private void ShowLegalMoves()
    {
        // Clear previous highlights
        foreach (Position boardPos in gameState.LegalMoves.Keys)
        {
            Vector3 scenePos = BoardToScenePosition(boardPos) + Vector3.up * 0.1f;
            GameObject highlight = Instantiate(highlightPrefab, scenePos, Quaternion.identity);
            highlights.Add(highlight);
        }
    }

    private void HideLegalMoves()
    {
        highlights.ForEach(Destroy);
        highlights.Clear();
    }

    private void OnBoardClicked(Position boardPos)
    {
        if (gameState.MakeMove(boardPos, out MoveInfo moveInfo))
        {
            StartCoroutine(OnMoveMade(moveInfo));
        }
    }

    private System.Collections.IEnumerator OnBoardClickedCoroutine(Position boardPos)
    {
        if (gameState.MakeMove(boardPos, out MoveInfo moveInfo))
        {
            yield return OnMoveMade(moveInfo);
        }
    }

    private System.Collections.IEnumerator OnMoveMade(MoveInfo moveInfo)
    {
        HideLegalMoves(); // Hide legal moves after a move is made
        yield return ShowMove(moveInfo);
        yield return ShowTurnOutcome(moveInfo); // Show the outcome of the turn
        ShowLegalMoves(); // Show legal moves for the next player

        isTurnInProgress = false; // Mark the turn as complete
    }

    private Position SceneToBoardPosition(Vector3 scenePosition)
    {
        int col = (int)(scenePosition.x - 0.25f);
        int row = 7 - (int)(scenePosition.z - 0.25f);
        return new Position(row, col);
    }

    private Vector3 BoardToScenePosition(Position boardPos)
    {
        return new Vector3(boardPos.Col + 0.75f, 0, 7 - boardPos.Row + 0.75f);
    }

    private void SpawnDisc(Disc prefab, Position boardPos)
    {
        Vector3 scenePos = BoardToScenePosition(boardPos) + Vector3.up * 0.1f;
        discs[boardPos.Row, boardPos.Col] = Instantiate(prefab, scenePos, Quaternion.identity);
    }

    private void AddStartDiscs()
    {
        foreach (Position boardPos in gameState.OccupiedPositions())
        {
            Player player = gameState.Board[boardPos.Row, boardPos.Col];
            SpawnDisc(discPrefabs[player], boardPos);
        }
    }

    private void FlipDiscs(List<Position> positions)
    {
        foreach (Position boardPos in positions)
        {
            discs[boardPos.Row, boardPos.Col].Flip();
        }
    }

    private System.Collections.IEnumerator ShowMove(MoveInfo moveInfo)
    {
        SpawnDisc(discPrefabs[moveInfo.Player], moveInfo.Position);
        yield return new WaitForSeconds(0.33f);
        FlipDiscs(moveInfo.Outflanked);
        yield return new WaitForSeconds(0.83f);
    }

    private System.Collections.IEnumerator ShowTurnSkipped(Player skippedPlayer)
    {
        uiManager.SetSkippedText(skippedPlayer);
        yield return uiManager.AnimateTopText();
    }

    private System.Collections.IEnumerator ShowTurnOutcome(MoveInfo moveInfo)
    {
        if (gameState.GameOver)
        {
            yield return ShowGameOver(moveInfo.Player);
            yield break;
        }

        Player currentPlayer = gameState.CurrentPlayer;

        if (currentPlayer == moveInfo.Player)
        {
            yield return ShowTurnSkipped(currentPlayer.Opponent());
        }

        uiManager.SetPlayerText(currentPlayer);
    }

    private System.Collections.IEnumerator ShowGameOver(Player winner)
    {
        uiManager.SetTopText($"Neither Player Can Move!");
        yield return uiManager.AnimateTopText();

        yield return uiManager.ShowScoreText();
        yield return new WaitForSeconds(0.5f);

        yield return ShowCounting();
        uiManager.SetWinnerText(winner);
        yield return uiManager.ShowEndScreen();
    }

    private System.Collections.IEnumerator ShowCounting()
    {
        int black = 0, white = 0;
        foreach (Position boardPos in gameState.OccupiedPositions())
        {
            Player player = gameState.Board[boardPos.Row, boardPos.Col];
            if (player == Player.Black)
            {
                black++;
                uiManager.SetBlackScoreText(black);
            }
            else if (player == Player.White)
            {
                white++;
                uiManager.SetWhiteScoreText(white);
            }

            discs[boardPos.Row, boardPos.Col].Twitch();
            yield return new WaitForSeconds(0.05f);
        }
    }

    private System.Collections.IEnumerator RestartGame()
    {
        yield return uiManager.HideEndScreen();
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }

    public void OnPlayAgainButtonClicked()
    {
        StartCoroutine(RestartGame());
    }
}

