/**
 * Server 2
 * Utualizes Linux commands to handle multiple clients
 *      - Master Socket
 *      - List of connections
 *      - Pairing system : peerLookUp
 * 		- Saving Users 	 : userLookUp
 * SOURCE: https://www.geeksforgeeks.org/socket-programming-in-cc-handling-multiple-clients-on-server-without-multi-threading/
 */ 
#include <stdio.h> 
#include <string.h> //strlen 
#include <stdlib.h> 
#include <errno.h> 
#include <unistd.h> //close 
#include <arpa/inet.h> //close 
#include <sys/types.h> 
#include <sys/socket.h> 
#include <netinet/in.h> 
#include <sys/time.h> //FD_SET, FD_ISSET, FD_ZERO macros 
#include <bits/stdc++.h> 
#include <vector>
	
#define TRUE 1 
#define FALSE 0 
#define PORT 6007
#define MAXCLIENTS 30

using namespace std;

int opt = TRUE; 
int master_socket , addrlen , new_socket , client_socket[MAXCLIENTS] , 
	max_clients = 30 , activity, i , valread , sd, num_clients; 
int max_sd; 
struct sockaddr_in address; 
// socket : username
map<int, char*> userLookUp;
// socket : socket
map<int, int> peerLookUp;
	
char buffer[1025]; //data buffer of 1K 
	
// set of socket descriptors 
fd_set readfds; 

vector<char*> charToVector(char* buff)
{
	// Save split to tmp variable
	char* tmp = new char[strlen(buff)];
	strcpy(tmp, buff);
	char* s = strtok(tmp, "| \0");
	// Going to vector
	vector<char*> v;

	while (s != NULL)
	{
		v.push_back(s);
		//printf("Split: %s \n", s);
		s = strtok (NULL, "| \0");
	}
	return v;
}

void printUsers()
{
	for (const auto &pair : userLookUp) {
        printf("%d : %s \n", pair.first, pair.second);
    }
	for (const auto &pair : peerLookUp) {
        printf("%d : %d \n", pair.first, pair.second);
    }
}
// fetches username via userLookUp
// returns string, otherwise null
char* getUser(int fd)
{
	if(userLookUp.find(fd) != userLookUp.end())
		return userLookUp[fd];

	return (char *)"N/A";
}

void setUser(int fd, char* name)
{
	char* toAdd = new char[strlen(name)];
	strcpy(toAdd, name);
	printf("New User! %d:%s \n", fd, name, &name);
	userLookUp[fd] = toAdd;
	printUsers();
}

// fetches opponent socket via peerLookUp
// returns int, otherwise -1
int getOpponent(int fd)
{
	if(peerLookUp.find(fd) != peerLookUp.end())
		return peerLookUp.at(fd);
	return -1;
}
// update map of key to value
void setOpponent(int key, int value)
{
	if(key != -1)
	{
		printf("New Lobby Created: %i & %i \n", key, value);
        peerLookUp.emplace(key, value); // allow replacement
        printUsers();
	}
}

// send msg to opponent
void sendToOppenent(int fd, char* msg)
{
	int opp = getOpponent(fd);
	if(opp != -1)
	{
		printf("Sent: %s to %s \n", msg, getUser(opp));
		send(opp , msg , strlen(msg) , 0 ); 
	}
}

void StartGame(int fd)
{
	int opp = getOpponent(fd);
	printf("User	: %s at fd: %d \n", getUser(fd), fd);
	printf("Opponent: %s at fd: %d \n", getUser(opp), opp);
	if(opp != -1)
	{
		// Send fd color and opponent username
		char w[80];
		strcpy(w, "START|WHITE|");
		strcat(w, getUser(opp));
		// Send opponent color and my username
		char b[80];
		strcpy(b, "START|BLACK|");
		strcat(b, getUser(fd));

		printf("Sent: %s \n", w);
		send(fd , w , strlen(w) , 0 ); 
		printf("Sent: %s \n", b);
		send(opp , b , strlen(b) , 0 ); 
	}
}
		
void parseHeader(int fd, char* buffer)
{
	vector<char*> content = charToVector(buffer);
	char* cmd = content[0];
	//printf("Content: %s, %s \n", content[0], content[1]);

	switch(cmd[0])
    {
        case 'U':   // USER|xxx
			// Add to user to dictionary
			setUser(fd, content[1]);
			// send data to opponent
			StartGame(fd);
            break;
        case 'M':   // MOVE|x1|y1|x2|y2
		case 'C':   // CHAT|xxxxxxxxxxxxxxxxxxxxxxx
			sendToOppenent(fd, buffer);
            break;
        default:    // ignore
            string r2 = "NOT SUPPORTED";
            send(fd, r2.c_str(), strlen(r2.c_str()), 0);
            break;
    }
}

void closeClient(int sd, int i)
{
	num_clients--;
	// Remove sd from data structures
	peerLookUp.erase(sd);
	userLookUp.erase(sd);
	//Close the socket and mark as 0 in list for reuse 
	close( sd ); 
	client_socket[i] = 0; 
}

	
int main(int argc , char *argv[]) 
{ 
	
	// intialize all client_socket[] to 0 so not checked 
	for (i = 0; i < max_clients; i++) 
	{ 
		client_socket[i] = 0; 
	} 
		
	// create a master socket 
	if( (master_socket = socket(AF_INET , SOCK_STREAM , 0)) == 0) 
	{ 
		perror("socket failed"); 
		exit(EXIT_FAILURE); 
	} 
	
	// set master socket to allow multiple connections , 
	// this is just a good habit, it will work without this 
	if( setsockopt(master_socket, SOL_SOCKET, SO_REUSEADDR, (char *)&opt, 
		sizeof(opt)) < 0 ) 
	{ 
		perror("setsockopt"); 
		exit(EXIT_FAILURE); 
	} 
	
	//type of socket created 
	address.sin_family = AF_INET; // TCP
	address.sin_addr.s_addr = INADDR_ANY; // IPv4 | IPv6
	address.sin_port = htons( PORT ); 
		
	//bind the socket to localhost port 6007 
	if (bind(master_socket, (struct sockaddr *)&address, sizeof(address))<0) 
	{ 
		perror("bind failed"); 
		exit(EXIT_FAILURE); 
	} 
	printf("Listener on port %d \n", PORT); 
		
	//try to specify maximum of 3 pending connections for the master socket 
	if (listen(master_socket, 3) < 0) 
	{ 
		perror("listen"); 
		exit(EXIT_FAILURE); 
	} 
		
	//accept the incoming connection 
	addrlen = sizeof(address); 
	puts("Waiting for connections ..."); 
	num_clients = 0;
	while(true) 
	{ 
		//clear the socket set 
		FD_ZERO(&readfds); 
	
		//add master socket to set 
		FD_SET(master_socket, &readfds); 
		max_sd = master_socket; 
			
		//add child sockets to set 
		for ( i = 0 ; i < max_clients ; i++) 
		{ 
			//socket descriptor 
			sd = client_socket[i]; 
				
			//if valid socket descriptor then add to read list 
			if(sd > 0) 
				FD_SET( sd , &readfds); 
				
			//highest file descriptor number, need it for the select function 
			if(sd > max_sd) 
				max_sd = sd; 
		} 
	
		//wait for an activity on one of the sockets , timeout is NULL , 
		//so wait indefinitely 
		activity = select( max_sd + 1 , &readfds , NULL , NULL , NULL); 
	
		if ((activity < 0) && i < max_clients) 
		{ 
			printf("select error at: sd: %d with %d at: %d\n", max_sd + 1, client_socket[i], i); 
		} 
			
		//If something happened on the master socket , 
		//then its an incoming connection 
		if (FD_ISSET(master_socket, &readfds)) 
		{ 
			if ((new_socket = accept(master_socket, 
					(struct sockaddr *)&address, (socklen_t*)&addrlen))<0) 
			{ 
				perror("accept \n"); 
				exit(EXIT_FAILURE); 
			} 
			
			// inform user of socket number - used in send and receive commands 
			printf("New connection , socket fd is %d , ip is : %s , port : %d \n" ,
                new_socket , inet_ntoa(address.sin_addr) , ntohs 
				(address.sin_port)); 
				
			num_clients++;

			//add new socket to array of sockets 
			for (i = 0; i < max_clients; i++) 
			{ 
				//if position is empty 
				if( client_socket[i] == 0 ) 
				{ 
					client_socket[i] = new_socket; 
					printf("Adding to list of known sockets as %d of %d clients\n" , i, num_clients); 
					// Pair every even client with opponent
                    if(num_clients % 2 == 0)
                    {
                        // We have an even number of players
						// Check for next opponent
						// find opponent where client[j] != 0 && peerLookUp(sd) == -1
						for(int j = 0; j < max_clients; j++)
						{
							// don't pair with myself || any client sockets ( >= 4 )
							if(client_socket[j] != new_socket && client_socket[j] > 3)
							{
                                // un paired, alive socket waiting to play
								if(getOpponent(client_socket[j]) == -1)
								{
                                    // set both to each other
                                    setOpponent(new_socket, client_socket[j]);
                                    setOpponent(client_socket[j], new_socket);
								}
							}
						}
                        
                    }
					break; 
				} 
			} 
		} 
			
		//else its some IO operation on some other socket 
		for (i = 0; i < max_clients; i++) 
		{ 
			sd = client_socket[i]; 
		
            // Incoming message from sd
			if (FD_ISSET( sd , &readfds)) 
			{ 
				//Check if it was for closing , and also read the 
				//incoming message 
				if ((valread = read( sd , buffer, 1024)) == 0) 
				{ 
					//Somebody disconnected , get his details and print 
					getpeername(sd , (struct sockaddr*)&address , 
						(socklen_t*)&addrlen); 
					printf("%s disconnected , ip %s , port %d \n" , getUser(sd),
						inet_ntoa(address.sin_addr) , ntohs(address.sin_port)); 
					
					// Let opponenet know of disconnection
					sendToOppenent(sd, "QUIT");
					setOpponent(sd, -1);
					closeClient(sd, i);				
				} 
					
				// Echo back the message that came in 
				else
				{ 
					//set the string terminating NULL byte on the end 
					//of the data read 
					buffer[valread] = '\0'; 
					printf("Recieved: %s \n", buffer);
					parseHeader(sd, buffer);
				} 
			} 
		} 
	} 
		
	return 0; 
} 
