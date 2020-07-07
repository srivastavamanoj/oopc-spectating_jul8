import os
from slack import WebClient
from slack.errors import SlackApiError


client = WebClient(token=os.environ['SLACK_API_TOKEN'])


def send_message(channel, txt):
    try:
        response = client.chat_postMessage(
            channel=channel,
            text=txt)

    except SlackApiError as e:
        assert e.response["ok"] is False
        assert e.response["error"]  # str like 'invalid_auth', 'channel_not_found'
        print(f"Got an error: {e.response['error']}")


def send_file(channel, file_path):
    try:
        response = client.files_upload(
            channels=channel,
            file=file_path)

    except SlackApiError as e:
        assert e.response["ok"] is False
        assert e.response["error"]  # str like 'invalid_auth', 'channel_not_found'
        print(f"Got an error: {e.response['error']}")


def list_team_members():
    """
    Returns a list of paginated user objects, in no particular order
    """
    users = []
    try:
        response = client.users_list()
        users = response["members"]
        user_ids = list(map(lambda u: u["id"], users))

    except SlackApiError as e:
        print(f"Got an error: {e.response['error']}")

    return users, user_ids


def list_public_channels():
    list_channels = []
    try:
        response = client.conversations_list(types="public_channel")

    except SlackApiError as e:
        print(f"Got an error: {e.response['error']}")

    return list_channels



def join_channel(channel):
    try:
        response = client.conversations_join(channel=channel)

    except SlackApiError as e:
        print(f"Got an error: {e.response['error']}")


def leave_channel(channel):
    try:
        response = client.conversations_leave(channel=channel)

    except SlackApiError as e:
        print(f"Got an error: {e.response['error']}")


if __name__ == '__main__':
    channel = '#test-slack-api'
    #channel = 'U016TKCD944'
    msg = 'Here is a link: https://www.youtube.com/watch?v=MNWC4NMWfe8'
    send_message(channel, msg)

    # filepath = '../data/goal.jpg'
    # send_file(channel, filepath)

    # list_members, ids = list_team_members()
    # for member, id in zip(list_members, ids):
    #     print('{}  {}'.format(member['name'], member['id']))

