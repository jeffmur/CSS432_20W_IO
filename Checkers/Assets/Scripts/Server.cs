using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Server
{

    private static readonly int portNumber = 6969;
    static void Main(string[] args)
    {
        IPHostEntry host = Dns.GetHostEntry("localhost");
        IPAddress ipAddress = host.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, portNumber);

        try
        {

            // Create a Socket that will use Tcp protocol      
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // A Socket must be associated with an endpoint using the Bind method  
            listener.Bind(localEndPoint);

            // Specify how many requests a Socket can listen before it gives Server busy response.  
            listener.Listen(100);

            Console.WriteLine("Ready for connections...");
            while (true)
            {
                Socket handler = listener.Accept();
                Console.WriteLine("Connection request received");
                EndPoint client = handler.RemoteEndPoint;
                Console.WriteLine("Client details: " + client.ToString());
                Thread thread = new Thread(Server.ThreadProc);
                thread.Start(client);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public static void ThreadProc(object client)
    {
        Console.WriteLine("Thread created");
    }
}
