import socket
import sys

players = []
ERROR = "Invalid user"
HOST = "HOST"
JOIN = "JOIN "

def handlePlayers(user):
    for i in players:
        # refuse similar usernames or maybe same IP address
        if(i[0] == user): # or i[1] == user[1]):
            return ERROR
    print(players)
    if len(players) == 0:
        players.append((data, client_address[0]))
        print(players)
        return HOST

    toJoin = players.pop()
    return JOIN+str(toJoin)

# Create a TCP/IP socket
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# Bind the socket to the port
server_address = ('10.0.0.4', 6007)
print >>sys.stderr, 'starting up on %s port %s' % server_address
sock.bind(server_address)

# Listen for incoming connections
sock.listen(1)

while True:
    # Wait for a connection
    print >>sys.stderr, 'waiting for a connection'
    connection, client_address = sock.accept()
    try:
        print >>sys.stderr, 'connection from', client_address

        # Receive the data in small chunks and retransmit it
        data = connection.recv(16)
        print >>sys.stderr, 'received "%s"' % data
        if data:
            r = handlePlayers(data)
            print("Sent: " + r)
            connection.sendall(r)
    finally:
        # Clean up the connection
        connection.close()