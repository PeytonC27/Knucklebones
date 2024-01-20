using TMPro;
using UnityEngine;

public class EndScreenManager : MonoBehaviour
{
    TMP_Text winText;
    TMP_Text statText;
    GameObject restartButton;
    GameObject darkness;

    // Start is called before the first frame update
    void Start()
    {
        darkness = transform.GetChild(0).gameObject;
        winText = transform.GetChild(1).GetComponent<TMP_Text>();
        statText = transform.GetChild(2).GetComponent<TMP_Text>();
        restartButton = transform.GetChild(3).gameObject;
    }

    public void DisableEndScreen()
    {
        winText.text = "";
        statText.text = "";
        restartButton.SetActive(false);
        darkness.SetActive(false);
    }

    public void EnableEndScreen(string winner, int playerScore, int opponentScore)
    {
        winText.text = winner + " wins!";
        statText.text = "You <color=#77b3fe>" + playerScore + "</color> - <color=#dd5b71>" + opponentScore + "</color> Opponent";
        restartButton.SetActive(true);
        darkness.SetActive(true);
    }
}
