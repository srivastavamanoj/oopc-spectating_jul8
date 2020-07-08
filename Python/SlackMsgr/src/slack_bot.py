import os
from slack import WebClient
from slack.errors import SlackApiError


client = WebClient(token=os.environ['SLACK_API_TOKEN'])


def send_message(a_channel, txt):
    """Send a message to a channel or to a user.
    channel example: '#test-slack-api'
    user example: U016TKCD944
    """
    try:
        response = client.chat_postMessage(
            channel=a_channel,
            text=txt)

    except SlackApiError as e:
        print(f"Got an error: {e.response['error']}")


def send_file(a_channel, file_path):
    try:
        response = client.files_upload(
            channels=a_channel,
            file=file_path)

    except SlackApiError as e:
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


def join_channel(a_channel):
    try:
        response = client.conversations_join(channel=a_channel)

    except SlackApiError as e:
        print(f"Got an error: {e.response['error']}")


def leave_channel(a_channel):
    try:
        response = client.conversations_leave(channel=a_channel)

    except SlackApiError as e:
        print(f"Got an error: {e.response['error']}")


def main():
    # Tests
    channel = '#test-slack-api'
    # channel = 'U016TKCD944'
    msg = 'Here is a link: https://www.youtube.com/watch?v=R_ZxREUJQeQ'
    send_message(channel, msg)

    # file_path = '../data/goal.jpg'
    # send_file(channel, file_path)

    # list_members, ids = list_team_members()
    # for member, id in zip(list_members, ids):
    #     print('{}  {}'.format(member['name'], member['id']))

    print ('Program finished...')


if __name__ == '__main__':
    main()

