import socket
import slack_bot
import json


# Create UDP Socket to receive messages. Accept all connections
UDP_IP_RECEIVE = ''
UDP_PORT_RECEIVE = 9089
sock_receive = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)     # Internet, UDP
sock_receive.bind((UDP_IP_RECEIVE, UDP_PORT_RECEIVE))

# Create UDP Socket to send
UDP_IP_SEND = "127.0.0.1"
UDP_PORT_SEND_T1 = 9091         # Local team: Barza
UDP_PORT_SEND_T2 = 9092         # Visiting team: Real Madrid
sock_send = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)        # Internet, UDP


def listen_udp_msgs():
    # Suspend until data is received
    print('Waiting for an udp message -----------')
    data, addr = sock_receive.recvfrom(1024)
    msg_str = data.decode()
    print('Received UDP message: ', msg_str, ' - Address: ', addr)
    msg_dic = json.loads(msg_str)
    process_msg(msg_dic)


def process_msg(msg_dic):
    if msg_dic['eventType'] == 'goal':
        on_goal(msg_dic)

    elif msg_dic['eventType'] == 'file_ready':
        on_file_ready(msg_dic)

    elif msg_dic['eventType'] == 'corner':
        on_corner(msg_dic)

    elif msg_dic['eventType'] == 'goal_kick':
        on_goal_kick(msg_dic)


def on_goal(msg_dic):
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


def on_file_ready(msg_dic):
    pass


def on_corner(msg_dic):
    p_conceded = msg_dic['playerConcededCorner']
    t_awarded_corner = msg_dic['teamAwardedCorner']
    p_throw_in = msg_dic['playerToThrowIn']

    msg = 'A corner kick was conceded by {} who sent the ball out ' \
          'of play. {} has the opportunity now, and {} will take ' \
          'the corner kick. '\
        .format(p_conceded, t_awarded_corner, p_throw_in, )

    channel = '#test-slack-api'
    slack_bot.send_message(channel, msg)


def on_goal_kick(msg_dic):
    p_conceded_gk = msg_dic['playerConcededGk']
    t_awarded_gk = msg_dic['teamAwardedGk']
    gk_throw_in = msg_dic['goalKeeperToAct']

    msg = 'A goal kick was given to {}. {} sent the ball out ' \
          'of play. {} is getting ready to take the goal kick'.\
        format(t_awarded_gk, p_conceded_gk, gk_throw_in)

    channel = '#test-slack-api'
    slack_bot.send_message(channel, msg)


def send_unity(ip, port, txt):
    sock_send.sendto(bytes(txt, "utf-8"), (ip, port))


def send_msg_team1(txt):
    send_unity(UDP_IP_SEND, UDP_PORT_SEND_T1, txt)


def send_msg_team2(txt):
    send_unity(UDP_IP_SEND, UDP_PORT_SEND_T2, txt)


def main():
    while True:
        listen_udp_msgs()


if __name__ == '__main__':
    main()