using TMPro;
using UnityEngine;

public class EndScreenManager : MonoBehaviour
{
    GameObject darkness;
    TMP_Text winText;
    GameObject restartButton;
    GameObject mainMenuButton;

    // Start is called before the first frame update
    void Start()
    {
        darkness = transform.GetChild(0).gameObject;
        winText = transform.GetChild(1).GetComponent<TMP_Text>();
        restartButton = transform.GetChild(2).gameObject;
        mainMenuButton = transform.GetChild(3).gameObject;
    }

    public void DisableEndScreen()
    {
        winText.text = "";
        restartButton.SetActive(false);
        darkness.SetActive(false);
        mainMenuButton.SetActive(false);
    }

    public void EnableEndScreen(string winner, int playerScore, int opponentScore)
    {
        winText.text = winner + ((winner == "You") ? " win!" : " wins!");
        restartButton.SetActive(true);
        darkness.SetActive(true);
        mainMenuButton.SetActive(true);
    }
}
