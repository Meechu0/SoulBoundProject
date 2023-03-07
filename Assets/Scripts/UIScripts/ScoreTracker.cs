using UnityEngine;
using UnityEngine.UI;
public class ScoreTracker : MonoBehaviour
{
    public Transform player;
    public Text P1Score;
    public Text P2Score;

    public int Player1Score;
    public int Player2Score;

    public GameObject WinState;
    public Text Results;
    public GameObject p1Sprite;
    public GameObject p2Sprite;



    public bool Player1Turn = true;

    
    void Update()
    {

        if (Player1Turn == true)
        {
            P1Score.text = Player1Score.ToString();

            Player1Score = (int)GameObject.FindWithTag("Player").transform.position.x - Player2Score;
            if (Player1Score <= 0)
            {
                Player1Score = 0;
                P1Score.text = "0";
            }
        }
        else
        {
            P2Score.text = Player2Score.ToString();

            Player2Score = (int)GameObject.FindWithTag("Player").transform.position.x - Player1Score;
            if (Player2Score <= 0)
            {
                Player2Score = 0;
                P2Score.text = "0";
            }
        }

        if (Player1Score >= 500)
        {
            p1Sprite.SetActive(true);
            p2Sprite.SetActive(false);
            Results.text = ("Player 1 Wins!");
            WinState.SetActive(true);
            Time.timeScale = 0;
        }
        else if (Player2Score >= 500)
        {
            p1Sprite.SetActive(false);
            p2Sprite.SetActive(true);
            Results.text = ("Player 2 Wins!");
            WinState.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            p1Sprite.SetActive(false);
            p2Sprite.SetActive(false);
            WinState.SetActive(false);
        }
    }
}