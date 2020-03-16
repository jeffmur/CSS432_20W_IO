using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WinnerBanner : MonoBehaviour
{
    private GameObject winnerText;
    private GameObject winnerPanel;
    void Start()
    {
        winnerPanel = transform.GetChild(0).gameObject;
        winnerText = transform.GetChild(1).gameObject;
        winnerPanel.SetActive(false);
        winnerText.SetActive(false);
    }

    public void DisplayWinner(string winner)
    {
        winnerPanel.SetActive(true);
        winnerText.SetActive(true);
        winnerText.GetComponent<Text>().text = winner + " Wins!";
    }

    public void DisableDisplay()
    {
        winnerPanel.SetActive(false);
        winnerText.SetActive(false);
    }
}
