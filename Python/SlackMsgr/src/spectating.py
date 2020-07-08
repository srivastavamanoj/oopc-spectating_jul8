import socket
import slack_bot
import json
import threading
from pynput import keyboard
import unity_comm as unity
import video_uploader_comm as uploader


def listen_unity_msgs():
    while True:
        unity.listen_udp_msgs()


def listen_youtube_uploader():
    while True:
        uploader.listen_event_msgs()


def on_press(key):
    if key == keyboard.Key.esc:
        return False  # stop listener
    try:
        k = key.char  # single-char keys
    except:
        k = key.name  # other keys

    # Tests
    if k in ['1', '2', '3']:  # keys of interest
        print('Key pressed: ' + k)

        # Test sending msgs to Unity
        txt_team1 = 'Go Barza !!!'
        txt_team2 = 'Go Real Madrid !!!'
        if k == '1':
            unity.send_msg_team1(txt_team1)
        elif k == '2':
            unity.send_msg_team2(txt_team2)

        # Test notifying that the video has been uploaded to YouTube
        elif k == '3':
            msg = 'ready https://www.youtube.com/watch?v=R_ZxREUJQeQ'
            uploader.set_event_msg(msg)

def main():
    t_unity = threading.Thread(target=listen_unity_msgs, daemon=True)
    t_unity.start()

    t_youtube = threading.Thread(target=listen_youtube_uploader, daemon=True)
    t_youtube.start()

    key_listener = keyboard.Listener(on_press=on_press)
    key_listener.start()

    while True:
        pass

    print ('Program finished...')



if __name__ == '__main__':
    main()