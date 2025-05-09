using UnityEngine;

public class Disc : MonoBehaviour
{
    [SerializeField] private Player up;

    private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Flip()
    {
        if (up == Player.Black)
        {
            animator.Play("BlackToWhite");
            up = Player.White;
        }
        else
        {
            animator.Play("WhiteToBlack");
            up = Player.Black;
        }
    }

    public void Twitch()
    {
        animator.Play("TwitchDisc");
    }
}
