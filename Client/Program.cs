using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    public class Program
    {
        private static readonly int portNumber = 6007;
        private static string serverAddress = "70.37.69.170";
        private static byte[] buffer = new byte[256];
        private static string opponentUsername;
        private static byte[] opponentIpAddresss;

        public static void Main(string[] args)
        {
            // create IPAddress object from ip address
            if (!IPAddress.TryParse(serverAddress, out var serverIp))
            {
                Console.WriteLine("Invalid IP Address");
                return;
            }
                
            IPEndPoint serverEndPoint = new IPEndPoint(serverIp, portNumber);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            socket.Connect(serverEndPoint);

            if (!socket.Connected)
            {
                Console.WriteLine("Couldn't connect to server");
                return;
            }
            
            // connected to server, input unique username
            Console.Write("Input Unique Username: ");
            string username = Console.ReadLine();
            if (username != null)
                buffer = Encoding.ASCII.GetBytes(username);
            
            // send username to server
            socket.Send(buffer, buffer.Length, 0);
            buffer = new byte[256];
            
            // read response from server
            while (socket.Receive(buffer, buffer.Length, 0) == 0) {}

            string data = Encoding.ASCII.GetString(buffer);
            Console.WriteLine("Received: " + data);
            
            if (data.Contains("HOST"))
            {
                // create new socket and wait for client connection
                // receive client IP and username
            }
            else if (data.Contains("JOIN"))
            {
                ExtractTokens(data);
                // get the IP address and name of HOST and connect
                // send username and IP
            }
            else // invalid username
            {
                Console.WriteLine("Error: " + data);
            }
            
            socket.Close();
        }

        private static void ExtractTokens(string data)
        {
            string[] tokens = data.Split('\'');

            // get username of host
            opponentUsername= tokens[1];
            
            // get IP address of host
            opponentIpAddresss = Encoding.ASCII.GetBytes(tokens[3]);
        }
    }
}