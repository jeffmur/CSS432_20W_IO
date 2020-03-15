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

    public GameObject alertBanner;
    // Start is called before the first frame update
    void Awake()
    {
        if (GameManager.Instance != null)
            client = GameManager.Instance.clientObject.GetComponent<Client>();
        else
            Debug.LogError("GameManager is null at start of Game!");

        Instance = GetComponent<GameStat>();
    }

    private void Start()
    {
        OpponentPopUp();
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

    public void OpponentPopUp()
    {
        opponent = GameManager.Instance.oponentUsername;
        whoAmI = client.clientName;
        // only show for online
        if (opponent != "" && whoAmI != "")
        {
            showDown.text = $"{whoAmI} vs {opponent}";
            ChatMessage($"has joined", false);
        }
        else
            showDown.text = "Practice";

        StartCoroutine(hideAfter(2));
    }

    // Local: White || Black Turn
    // Online: Your Move
    bool completeFade = true;
    public void ShowTurn(bool isPlaying)
    {
        /// CURRRENTLY FLASHING
        if (isPlaying && completeFade)
        {
            completeFade = false;
            showDown.text = "Your Move";
            StartCoroutine(FadeOut(2f, showDown, alertBanner.GetComponent<Image>()));
        }
    }


    private IEnumerator hideAfter(int sec)
    {
        // reset
        Image rend = alertBanner.GetComponent<Image>();
        Color temp = rend.color;
        temp.a = 1.0f;
        rend.color = temp;

        yield return new WaitForSeconds(sec);

        StartCoroutine(FadeOut(1f, showDown, alertBanner.GetComponent<Image>()));

    }

    public IEnumerator FadeOut(float t, Text i, Image j)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        j.color = new Color(j.color.r, j.color.g, j.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            j.color = new Color(j.color.r, j.color.g, j.color.b, j.color.a - (Time.deltaTime / t));
            yield return null;
        }
        completeFade = true;
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
}
