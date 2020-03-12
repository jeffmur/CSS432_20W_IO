using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Client : MonoBehaviour
{
    public GameManager gameManager;
    public string clientName;
    public bool isHost;
    private static readonly int portNumber = 6007;
    private static string serverAddress = "70.37.69.170";

    private bool socketReady;
    private Socket socket;
    private static byte[] buffer = new byte[256];


    enum GameHeaders
    {
        USER = 0,
        MOVE = 1,
        ENDT = 2,
        CHAT = 3
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    
    // connect to the game server
    public bool ConnectToServer()
    {
        if (socketReady) return false;

        // create IPAddress object from ip address
        if (!IPAddress.TryParse(serverAddress, out var serverIp))
        {
            Debug.LogError("Invalid IP Address");
        }

        try
        {
            IPEndPoint serverEndPoint = new IPEndPoint(serverIp, portNumber);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(serverEndPoint);

            Debug.Log("Client has connected");

            socketReady = true;
        }
        catch (Exception e)
        {
            Debug.LogError("Socket error: " + e.Message);
        }

        return socketReady;
    }
    // check socket to server for messages on every frame 
    void Update()
    {
        if (socketReady)
        {
            if (socket.Receive(buffer) > 0) // wait for data
            {
                string data = Encoding.ASCII.GetString(buffer);
                Debug.Log("Data received " + data);
                OnIncomingData(data);
                buffer = new byte[256];
            }
        }
    }

    // Send messages to the server
    public void Send(int header, string data)
    {
        if (!socketReady) return;

        string message = "";

        switch (header)
        {
            case (int)GameHeaders.USER:
                message = "USER|";
                break;
            case (int)GameHeaders.MOVE:
                message = "MOVE|";
                break;
            case (int)GameHeaders.ENDT:
                message = "ENDT|";
                break;
            case (int)GameHeaders.CHAT:
                message = "CHAT|";
                break;
        }
        message += data;
        buffer = Encoding.ASCII.GetBytes(message);
        socket.Send(buffer, buffer.Length, 0);
        buffer = new byte[256];
    }

    // Read messages from the server
    private void OnIncomingData(string data)
    {
        Debug.Log("Client: " + data);
        string[] aData = data.Split('|');

        switch (aData[0])
        {
            case "START":
                gameManager.oponentUsername = aData[1];
                Debug.Log("START, oponent name: " + gameManager.oponentUsername);
                gameManager.StartGame();
                break;
            case "MOVE":
                Debug.Log("MOVE");
                // move pieces
                break;
            case "ENDT":
                Debug.Log("ENDT");
                // end turn
                break;
            case "CHAT":
                Debug.Log("CHAT");
                // update chat log
                break;
            default:
                Debug.LogError("Received a header outside of range");
                break;
        }
    }

    // close socket on each instance of the game closing or a user quiting
    private void OnApplicationQuit()
    {
        CloseSocket();
    }
    private void OnDisable()
    {
        CloseSocket();
    }
    private void CloseSocket()
    {
        if (!socketReady) return;

        socket.Close();
        socketReady = false;
    }
}
