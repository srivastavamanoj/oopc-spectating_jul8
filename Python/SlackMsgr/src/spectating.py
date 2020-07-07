import socket
import slack_bot
import json

# Create UDP Socket to receive. Accept all connections
UDP_IP_RECEIVE = ''
UDP_PORT_RECEIVE = 9089
sock_receive = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)     # Internet, UDP
sock_receive.bind((UDP_IP_RECEIVE, UDP_PORT_RECEIVE))

# Create UDP Socket to send
UDP_IP_SEND = "127.0.0.1"
UDP_PORT_SEND = 9088
sock_send = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)        # Internet, UDP


def listen_Unity():
    while True:
        # Suspend until data is received
        print('Waiting for an udp message ------------------------------------------------')
        data, addr = sock_receive.recvfrom(1024)
        strReceived = data.decode()
        print('Received UDP message: ', strReceived, ' - Address: ', addr)
        msg_json = json.loads(strReceived)

        if msg_json['eventType'] == 'goal':
            slack_bot.send_message(strReceived)


if __name__ == '__main__':
    listen_Unity()