using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { set; get; }

    public GameObject mainMenu;
    public GameObject hostPrompt;
    public GameObject userPrompt;

    public InputField nameInput;

    public GameObject serverPrefab;
    public GameObject clientPrefab;

    private void Start()
    {
        Instance = this;
        hostPrompt.SetActive(false);
        userPrompt.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }

    public void ConnectButton()
    {
        mainMenu.SetActive(false);
        userPrompt.SetActive(true);
    }
    // Directing to HOSTING or JOINING
    public void HandlePeer()
    {
        Server s = GetComponent<Server>();
        int option = 0;// s.ResolveOpponent(nameInput.text);
        switch (option)
        {
            case 0:
                //HostServer();
                break;
            case 1:
                //ConnectToHost(s.opponentIpAddress);
                break;
            default:
                //s.ResolveOpponent(nameInput.text);
                break;
        }
    }

    public void HostServer()
    {
        string hostAddress = "127.0.0.1";
        try
        {
            Client c = Instantiate(clientPrefab).GetComponent<Client>();
            c.clientName = nameInput.text;
            c.isHost = true;
            if (c.clientName == "")
                c.clientName = "Host";
            c.ConnectToServer(hostAddress, 6321);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }

        mainMenu.SetActive(false);
        userPrompt.SetActive(false);
        hostPrompt.SetActive(true);
    }
    public void ConnectToHost(string hostAddress)
    {
        try
        {
            Client c = Instantiate(clientPrefab).GetComponent<Client>();
            c.clientName = nameInput.text;
            if (c.clientName == "")
                c.clientName = "Client";

            Debug.Log(hostAddress);
            c.ConnectToServer(hostAddress, 6321);
            userPrompt.SetActive(false);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
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
        SceneManager.LoadScene("Game");
    }
}

