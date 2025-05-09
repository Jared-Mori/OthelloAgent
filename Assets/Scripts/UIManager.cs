using UnityEngine;
using TMPro; // Importing TextMeshPro namespace for text rendering
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI topText; // Reference to the TextMeshProUGUI component for displaying score
    [SerializeField] TextMeshProUGUI blackScoreText;
    [SerializeField] TextMeshProUGUI whiteScoreText; // Reference to the TextMeshProUGUI component for displaying score
    [SerializeField] TextMeshProUGUI winnerText;
    [SerializeField] Image blackOverlay; // Reference to the GameObject for the game over panel
    [SerializeField] RectTransform playAgainButton; // Reference to the GameObject for the game over panel

    public void SetPlayerText(Player currentPlayer)
    {
        if (currentPlayer == Player.Black)
        {
            topText.text = "Black's Turn";
        }
        else if (currentPlayer == Player.White)
        {
            topText.text = "White's Turn";
        }
    }

    public void SetSkippedText(Player skippedPlayer)
    {
        if (skippedPlayer == Player.Black)
        {
            topText.text = "Black Cannot Move";
        }
        else if (skippedPlayer == Player.White)
        {
            topText.text = "White Cannot Move!";
        }
    }

    public IEnumerator AnimateTopText()
    {
        topText.transform.LeanScale(Vector3.one * 1.2f, 0.25f).setLoopPingPong(4);
        yield return new WaitForSeconds(2f);
    }

    public void SetTopText(string text)
    {
        topText.text = text;
    }

    private IEnumerator ScaleDown(RectTransform rect)
    {
        rect.LeanScale(Vector3.zero, 0.2f);
        yield return new WaitForSeconds(0.2f);
        rect.gameObject.SetActive(false);
    }

    private IEnumerator ScaleUp(RectTransform rect)
    {
        rect.gameObject.SetActive(true);
        rect.localScale = Vector3.zero;
        rect.LeanScale(Vector3.one, 0.2f);
        yield return new WaitForSeconds(0.2f);
    }

    public IEnumerator ShowScoreText()
    {
        yield return ScaleDown(topText.rectTransform);
        yield return ScaleUp(blackScoreText.rectTransform);
        yield return ScaleUp(whiteScoreText.rectTransform);
    }

    public void SetBlackScoreText(int score)
    {
        blackScoreText.text = $"Black: {score}";
    }

    public void SetWhiteScoreText(int score)
    {
        whiteScoreText.text = $"White: {score}";
    }

    private IEnumerator ShowOverlay()
    {
        blackOverlay.gameObject.SetActive(true);
        blackOverlay.color = Color.clear;
        blackOverlay.rectTransform.LeanAlpha(0.8f, 1f);
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator HideOverlay()
    {
        blackOverlay.rectTransform.LeanAlpha(0f, 1f);
        yield return new WaitForSeconds(1f);
        blackOverlay.gameObject.SetActive(false);
    }

    private IEnumerator MoveScoresDown()
    {
        blackScoreText.rectTransform.LeanMoveY(0f, 0.5f);
        whiteScoreText.rectTransform.LeanMoveY(0f, 0.5f);
        yield return new WaitForSeconds(0.5f);
    }

    public void SetWinnerText(Player winner)
    {
        switch (winner)
        {
            case Player.Black:
                winnerText.text = "Black Wins!";
                break;
            case Player.White:
                winnerText.text = "White Wins!";
                break;
            default:
                winnerText.text = "It's a Draw!";
                break;
        }
    }

    public IEnumerator ShowEndScreen()
    {
        yield return ShowOverlay();
        yield return MoveScoresDown();
        yield return ScaleUp(winnerText.rectTransform);
        yield return ScaleUp(playAgainButton);
    }

    public IEnumerator HideEndScreen()
    {
        StartCoroutine(ScaleDown(winnerText.rectTransform));
        StartCoroutine(ScaleDown(blackScoreText.rectTransform));
        StartCoroutine(ScaleDown(whiteScoreText.rectTransform));
        StartCoroutine(ScaleDown(playAgainButton));
        yield  return new WaitForSeconds(0.5f);
        yield return HideOverlay();
    }
}
