using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStat : MonoBehaviour
{
    public static GameStat Instance { get; set; }
    private Client client;

    public Text showDown;
    public string whoAmI;
    public string opponent;
    public InputField chatMessage;
    public GameObject messagePrefab;
    public GameObject chatPanel;
    public GameObject chat;
    public GameObject endPrompt;

    public GameObject alertBanner;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = GetComponent<GameStat>();
    }

    private void Start()
    {
        client = Client.Instance;
        if(GameManager.Instance.isOnline)
        {
            opponent = GameManager.Instance.oponentUsername;
            whoAmI = Client.Instance.clientName;
        }
        Banner.alert.OpponentPopUp(opponent, whoAmI);
    }

    // Update is called once per frame
    void Update()
    {
        // Enter text into chat w/ Enter
        if (Input.GetKeyDown(KeyCode.Return) && chatMessage.text != "")
            ChatMessage(chatMessage.text, true);

        // Toggle Chat w/ T
        if (Input.GetKeyDown(KeyCode.T) && chatMessage.text == "")
            ToggleChat();
    }

    private void ToggleChat()
    {
        if (chat.activeInHierarchy)
            chat.SetActive(false);
        else
            chat.SetActive(true);
    }

    // --------------------------- CHAT SYSTEM -----------------------------------
    public void ChatMessage(string message, bool send)
    {
        // set up message box
        GameObject m = Instantiate(messagePrefab);
        Text sentMessage = m.transform.GetChild(0).GetComponent<Text>();

        // send to CHAT to opponent
        if (send)
        {
            // local host check
            if (client.clientName != "")
                sentMessage.text = $"<color=green> {whoAmI} </color> {message}";
            else
                sentMessage.text = message;

            // SEND
            client.Send(3, message);
            chatMessage.text = "";
        }
        else // Recieved Message
        {
            sentMessage.text = $"<color=red> {opponent} </color> {message}";
        }

        // place in chatbox
        m.transform.SetParent(chatPanel.transform);

    }

    // --------------------------- END OF GAME -----------------------------------

    public void EndOfGame()
    {
        // Display quit or rematch
        endPrompt.SetActive(true);
    }

    public void toMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Rematch()
    {
        // Send rematch
        client.Send(2, "");
        // on recieve push a new game and flip sides
        SceneManager.LoadScene("Game");
    }
}
