using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Client : MonoBehaviour
{
    public string clientName;
    public bool isHost;

    private bool socketReady;
    private Socket socket;
    private static byte[] buffer = new byte[256];

    public List<GameClient> players = new List<GameClient>();

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public bool ConnectToServer(string host, int port)
    {
        if (socketReady) return false;
        // create IPAddress object from ip address
        if (!IPAddress.TryParse(host, out var serverIp))
        {
            Debug.LogError("Invalid IP Address");
        }
        try
        {
            IPEndPoint serverEndPoint = new IPEndPoint(serverIp, port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(serverEndPoint);

            Debug.Log("Client has connected");

            socketReady = true;
        }
        catch (Exception e)
        {
            Debug.LogError("Socket error " + e.Message);
        }

        return socketReady;
    }

    private void Update()
    {
        if (socketReady)
        {
            if (socket.Receive(buffer, buffer.Length, 0) != 0) // wait for data
            {
                string data = Encoding.ASCII.GetString(buffer);
                OnIncomingData(data);
                buffer = new byte[256];
            }
        }
    }

    // Send messages to the server
    public void Send(string data)
    {
        if (!socketReady) return;

        buffer = Encoding.ASCII.GetBytes(data);
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
            case "SWHO":
                for (int i = 1; i < aData.Length - 1; i++)
                {
                    UserConnected(aData[i], false);
                }
                Send("CWHO|" + clientName + "|" + ((isHost) ? 1 : 0).ToString());
                break;

            case "SCNN":
                UserConnected(aData[1], false);
                break;

            case "SMOV":
                //CheckerBoard.Instance.TryMove(int.Parse(aData[1]), int.Parse(aData[2]), int.Parse(aData[3]), int.Parse(aData[4]));
                break;

            case "SMSG":
                //CheckerBoard.Instance.ChatMessage(aData[1]);
                //CheckerBoard.Instance.ChatMessage(aData[1]);
                break;
        }
    }

    private void UserConnected(string name, bool host)
    {
        GameClient gc = new GameClient();
        gc.name = name;

        players.Add(gc);

        if (players.Count == 2)
            GameManager.Instance.StartGame();
    }

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

public class GameClient
{
    public string name;
    public bool isHost;
}
