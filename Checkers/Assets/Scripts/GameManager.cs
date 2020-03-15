using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { set; get; }

    public GameObject mainMenu;
    public GameObject hostPrompt;
    public GameObject userPrompt;
    public GameObject clientObject;

    public bool startTrigger = false;
    public byte[] incomingDataTrigger = null;
    public bool isOnline;
    public bool isWhite;
    public InputField nameInput;
    public string oponentUsername;

    private Client client;

    enum GameHeaders
    {
        USER = 0,
        MOVE = 1,
        ENDT = 2,
        CHAT = 3
    }

    private void Start()
    {
        Instance = this.GetComponent<GameManager>();
        isWhite = true;
        hostPrompt.SetActive(false);
        userPrompt.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (startTrigger)
            StartGame(); // this is very bad but works ¯\_(ツ)_/¯
                        // thank you threads :)
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
        isOnline = true;
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

    public void Quit()
    {
        // Close socket if online
        if (client != null)
            client.CloseSocket();
        SceneManager.LoadScene("Menu");
    }

    public void BackButton()
    {
        isOnline = false;
        mainMenu.SetActive(true);
        userPrompt.SetActive(false);
        hostPrompt.SetActive(false);
    }

    public void StartGame()
    {
        startTrigger = false;
        SceneManager.LoadScene("Game");
    }
}

