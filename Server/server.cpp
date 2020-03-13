/** -------------------------- Server.cpp --------------------------
 * @author Jeffrey Murray Jr
 * @class CSS 432 WI 20
 * @purpose read over incoming data over N number of times
 * @param serverPort repetitions
 * @see HW1-Report for more information 
 * @use ./runServer.sh
 * -----------------------------------------------------------------
*/  
#include <iostream>
#include <stdio.h>
#include <stdlib.h>
#include <sys/types.h>    // socket, bind
#include <sys/socket.h>   // socket, bind, listen, inet_ntoa
#include <netinet/in.h>   // htonl, htons, inet_ntoa
#include <arpa/inet.h>    // inet_ntoa
#include <netdb.h>        // gethostbyname
#include <unistd.h>       // read, write, close
#include <string.h>       // bzero
#include <netinet/tcp.h>  // TCP_NODELAY
#include <signal.h>
#include <unistd.h>
#include <fcntl.h>
#include <sys/time.h>
#include <pthread.h>

using namespace std;

char buffer[256]; // buffer used to receive messages

void clearBuffer()
{
    memset(buffer, 0, 256);
}

struct thread_data
{
    int Afd;
    string AuserName;
    int Bfd;
    string BuserName;
};


// creating thread pool
#define MAX_CONNECTIONS 10
const char* port = "6007";

// Adds username from buffer
void addUser(int size, char *buff, string &username)
{
    for(int i = 5; i < size; i++)
    {
        username += buff[i];
    }
}

/**
 * START|opponentUsername
 * Commands:
 *      MOVE
 *      ENDT
 * QUIT
 */ 
void StartGame(int fd, thread_data *data)
{
    if(data->AuserName == "" || data->BuserName == "")
        return;

    string r1 = ("START|BLACK|" + data->AuserName);
    send(data->Bfd, r1.c_str(), strlen(r1.c_str()), 0);
    cout << "Sent: " << r1 << endl;

    // Send to B
    string r2 = "START|WHITE|" + data->BuserName;
    send(data->Afd, r2.c_str(), strlen(r2.c_str()), 0);
    cout << "Sent: " << r2 << endl;
}

/** GAME HEADERS
 * USER | xxx
 * MOVE | xxx
 * CHAT | xxx
 */ 
void parseHeaders(char* buff, int size, int fd, thread_data *data)
{
    string cmd;
    for(int i = 0; i < 4; i ++)
    {
        cmd += buff[i];
    }

    switch(cmd[0])
    {
        case 'U':   // Data = USER|xxx
            if(fd == data->Afd)
                addUser(size, buff, data->AuserName);
            else
                addUser(size, buff, data->BuserName);
            StartGame(fd, data);
            break;
        case 'M':   // move
            //SendToOpponent(fd, data, buff);
            break;
        case 'C':   // chat
        case 'E':   // endt
            cout << "Someone has disconnected" << endl;
            break;
        default:    // ignore
            string r2 = "NOT SUPPORTED";
            send(fd, r2.c_str(), strlen(r2.c_str()), 0);
            break;

    }
    clearBuffer();
}

void *serverThreadFunc(void *input) {
    thread_data *data = ((struct thread_data *) input);
    cout << "Entered Game Lobby :)" << endl;
    // receive incoming usernames
    while (true) 
    {   
        // Recieving data from A
        int a = recv(data->Afd, buffer, sizeof(buffer), 0);
        if (a > 0) {
            cout << buffer << endl;
            parseHeaders(buffer, sizeof(buffer), data->Afd, data);
        }
        clearBuffer();

        // Recieving data from B
        int b = recv(data->Bfd, buffer, sizeof(buffer), 0);
        if (b > 0) {
            cout << buffer << endl;
            parseHeaders(buffer, sizeof(buffer), data->Bfd, data);
        }
        clearBuffer();
    }
}

/** main()
 * @param port, repetitions
 * Listens on desired port and reads n of repetitions from
 * incoming client requests. Each request is handled by a child process
*/ 
int main(int argc, char **argv) {

    if(argc != 1) // No arguments needed default port 6007
    {
        fprintf(stderr, "usage: %s \n", argv[0]);
        exit(1);
    }

    int s;
    int sock_fd = socket(AF_INET, SOCK_STREAM, 0);

    struct addrinfo hints, *result;
    memset(&hints, 0, sizeof (struct addrinfo));
    hints.ai_family = AF_INET;
    hints.ai_socktype = SOCK_STREAM;
    hints.ai_flags = AI_PASSIVE;

    s = getaddrinfo(NULL, port, &hints, &result);
    if (s != 0) {
        fprintf(stderr, "getaddrinfo: %s\n", gai_strerror(s));
        exit(1);
    }

	// allow socket to be reused
	const int yes = 1;
	setsockopt(sock_fd, SOL_SOCKET, SO_REUSEPORT, &yes, sizeof (yes));

    // Check if socket/port is in use
    if (bind(sock_fd, result->ai_addr, result->ai_addrlen) != 0) {
        perror("bind()");
        exit(1);
    }

    // Only allow up to 10 clients at a time
    if (listen(sock_fd, MAX_CONNECTIONS) != 0) {
        perror("listen()");
        exit(1);
    }
    
    struct sockaddr_in *clientAddr = (struct sockaddr_in *) result->ai_addr;
    socklen_t clientAddrSize = sizeof(clientAddr);
    printf("Listening on file descriptor %d, port %s\n", 
        sock_fd, 
        port);

	for(;;) // main accept() waiting for connections
	{
	    while (true) 
        {
            int firstSd = accept(sock_fd, (struct sockaddr *) &clientAddr, &clientAddrSize);
            cout << "Recieved First Connection!" << endl;
            // Await for second connection
            int secondSd = accept(sock_fd, (struct sockaddr *) &clientAddr, &clientAddrSize);
            cout << "Recieved Second Connection!" << endl;
            // create a new thread
            pthread_t newThread;
            struct thread_data *data = new thread_data;
            data->Afd = firstSd;
            data->Bfd = secondSd;
            int iret1 = pthread_create(&newThread, NULL, serverThreadFunc, (void *) data);
        }
	}

    return 0;
}


