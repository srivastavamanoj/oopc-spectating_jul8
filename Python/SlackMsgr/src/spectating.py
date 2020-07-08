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


def listen_unity():
    while True:
        # Suspend until data is received
        print('Waiting for an udp message -----------')
        data, addr = sock_receive.recvfrom(1024)
        msg_str = data.decode()
        print('Received UDP message: ', msg_str, ' - Address: ', addr)
        msg_dic = json.loads(msg_str)

        if msg_dic['eventType'] == 'goal':
            OnGoal(msg_dic)


def OnGoal(msg_dic):
    min = msg_dic['minute']
    sec = msg_dic['second']
    p_name = msg_dic['playerName']
    t_name = msg_dic['teamName']
    local_score = msg_dic['localTeamScore']
    visiting_score = msg_dic['visitingTeamScore']
    l_team_name = msg_dic['localTeamName']
    v_team_name = msg_dic['visitingTeamName']

    msg = 'Wow!!! {} just scored a goal for {} at minute {}:{}. ' \
          'The current score is ' '{} {} - {} {}'\
        .format(p_name, t_name, min, sec, l_team_name,
                local_score, v_team_name, visiting_score)

    channel = '#test-slack-api'
    slack_bot.send_message(channel, msg)


def main():
    listen_unity()


if __name__ == '__main__':
    main()