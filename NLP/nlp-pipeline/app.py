import json
import os
import threading

from flask import Flask, request, Response
from slack import WebClient
from slack.errors import SlackApiError

from state import State

# TODO: Get rid of this, should be set by an external script instead of code
os.environ['SLACK_API_TOKEN'] = 'xoxb-1210735252167-1224343562405-iPaYFMC9P4HouHAUpV30yib3'

SLACK_API_TOKEN = os.environ['SLACK_API_TOKEN']

TEAM_1, TEAM_2 = '#teamBlue', '#teamRed'

app = Flask(__name__)
client = WebClient(token=SLACK_API_TOKEN)
state = State(TEAM_1, TEAM_2)


def send_message(message):
    try:
        response = client.chat_postMessage(channel='#test-nlp', text=message)
        print('Outgoing data: ', response)
    except SlackApiError as e:
        print(e.response)


def loop():
    print('In the loop! Will send summaries if there exists...')
    summary_1, summary_2 = state.get_summary(TEAM_1), state.get_summary(TEAM_2)
    if summary_1:
        send_message(TEAM_1 + ' : ' + summary_1)

    if summary_2:
        send_message(TEAM_2 + ' : ' + summary_2)

    threading.Timer(60, loop).start()


@app.route('/', methods=['GET', 'POST'])
def slack_incoming():
    # return json.loads(request.data)['challenge']
    data = json.loads(request.data)
    print('Incoming data: ', data)

    if 'client_msg_id' in data['event']:
        content = data['event']['text']
        if TEAM_1 in content:
            state.add_message(TEAM_1, content)
            send_message(state.get_sentiment(TEAM_1))
        elif TEAM_2 in content:
            state.add_message(TEAM_2, content)
            send_message(state.get_sentiment(TEAM_2))

    return Response(), 200


@app.route('/state', methods=['GET', 'POST'])
def get_state():
    return state.cache


@app.route('/sentiment', methods=['GET', 'POST'])
def get_sentiment():
    return {TEAM_1: state.get_sentiment(TEAM_1), TEAM_2: state.get_sentiment(TEAM_2)}


@app.route('/summary', methods=['GET', 'POST'])
def get_summary():
    return {TEAM_1: state.get_summary(TEAM_1), TEAM_2: state.get_summary(TEAM_2)}


@app.route('/metrics', methods=['GET', 'POST'])
def get_metrics():
    return {TEAM_1: state.get_metrics(TEAM_1), TEAM_2: state.get_metrics(TEAM_2)}


loop()
