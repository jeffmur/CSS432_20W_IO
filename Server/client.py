import socket
import sys

# Create a TCP/IP socket
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# Connect the socket to the port where the server is listening
server_address = ('70.37.69.170', 6007)
print >>sys.stderr, 'connecting to %s port %s' % server_address
sock.connect(server_address)

try:
    
    # Send data
    message = 'Jeffrey'
    print >>sys.stderr, 'sending "%s"' % message
    sock.sendall(message)

    while True:
        data = sock.recv(16)
        if not data:
            break
        print >>sys.stderr, 'received "%s"' % data

finally:
    print >>sys.stderr, 'closing socket'
    sock.close()