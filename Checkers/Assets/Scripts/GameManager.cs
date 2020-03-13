using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { set; get; }

    public GameObject mainMenu;
    public GameObject hostPrompt;
    public GameObject userPrompt;
    public GameObject clientObject;

    public InputField nameInput;

    public string oponentUsername;

    private Client client;

    enum GameHeaders
    {
        USER = 0,
        MOVE = 1,
        CHAT = 2
    }

    private void Start()
    {
        hostPrompt.SetActive(false);
        userPrompt.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }
    public void PromptUsername()
    {
        mainMenu.SetActive(false);
        userPrompt.SetActive(true);
    }

    public void ConnectButton()
    {
        IniatlizeConnection();
        mainMenu.SetActive(false);
        userPrompt.SetActive(false);
    }


    // connect to server and send username
    public void IniatlizeConnection()
    {
        ConnectToServer();
        hostPrompt.SetActive(true);
    }

    // 
    public void ConnectToServer()
    {
        try
        {
            client = GameObject.Find("ClientObject").GetComponent<Client>();
            client.clientName = nameInput.text;
            client.isHost = true;
            if (client.clientName == "")
                client.clientName = "Anonymous";

            client.ConnectToServer();
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }

        mainMenu.SetActive(false);
        hostPrompt.SetActive(true);
    }

    public void BackButton()
    {
        mainMenu.SetActive(true);
        userPrompt.SetActive(false);
        hostPrompt.SetActive(false);

        //Host s = FindObjectOfType<Host>();
        GameObject s = null;
        if (s != null)
            Destroy(s.gameObject);

        Client c = FindObjectOfType<Client>();
        if (c != null)
            Destroy(c.gameObject);
    }

    public void StartGame()
    {
        Debug.Log("LOAD GAME");
        SceneManager.LoadScene("Game");
    }
}

