import threading
import slack_bot


SLACK_CHANNEL = '#test-slack-api'
e = threading.Event()
message = ''


def listen_event_msgs():
    global message
    event_is_set = e.wait()
    process_msg()
    e.clear()
    message = ''


def process_msg():
    global message
    words = message.split()
    status = ''
    url = ''
    if len(words) > 1:
        status = words[0]
        url = words [1]

    if status == 'ready':
        msg = 'The video is ready here is the link: ' + url
        slack_bot.send_message(SLACK_CHANNEL, msg)


def set_event_msg(txt):
    global message
    message = txt
    print ('received event message: {}'.format(message))
    e.set()