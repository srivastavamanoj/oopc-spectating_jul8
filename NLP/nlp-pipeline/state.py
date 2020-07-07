from transformers import pipeline

PIPELINE_SENTIMENT = 'sentiment-analysis'
PIPELINE_SUMMARIZE = 'summarization'

SUMMARIZER_MODEL = 't5-base'
SUMMARIZER_TOKENIZER = 't5-base'
SUMMARIZER_MIN_LEN = 10
SUMMARIZER_MAX_LEN = 30


class State:

    def __init__(self, team_a, team_b):
        self.cache = dict()
        self.cache[team_a] = []
        self.cache[team_b] = []

        self.sentiment_analyzer = pipeline(PIPELINE_SENTIMENT)
        self.summarizer = pipeline(PIPELINE_SUMMARIZE, model=SUMMARIZER_MODEL, tokenizer=SUMMARIZER_TOKENIZER)

    def add_message(self, team_name, message):
        message = message.replace(team_name, '')
        sentiment = self.sentiment_analyzer(message)[0]
        sentiment['message'] = message
        self.cache.get(team_name).append(sentiment)

    def get_content(self, team_name):
        return '.'.join([item['message'] for item in self.cache.get(team_name)]).strip()

    def get_summary(self, team_name):
        content = self.get_content(team_name)
        if content == '' or content is None:
            return None

        if len(content) > SUMMARIZER_MIN_LEN:
            return self.summarizer(content, min_length=SUMMARIZER_MIN_LEN, max_length=SUMMARIZER_MAX_LEN)[0][
                'summary_text']

        return None

    def get_metrics(self, team_name):
        positive_messages, negative_messages = [], []
        for item in self.cache.get(team_name):
            if item['label'] == 'POSITIVE':
                positive_messages.append(item['score'])
            else:
                negative_messages.append(item['score'])

        return {'positive_count': len(positive_messages),
                'negative_count': len(negative_messages),
                'positive_avg': 0 if len(positive_messages) == 0 else sum(positive_messages) / len(positive_messages),
                'negative_avg': 0 if len(negative_messages) == 0 else sum(negative_messages) / len(negative_messages)}

    def get_sentiment(self, team_name):
        content = self.get_content(team_name)
        if content == '' or content is None:
            return None

        return self.sentiment_analyzer(content)[0]


if __name__ == '__main__':
    TEAM_BLUE = '#teamBlue'
    TEAM_RED = '#teamRed'
    state = State(TEAM_BLUE, TEAM_RED)
    state.add_message(TEAM_BLUE, 'this is amazing #teamBlue')
    state.add_message(TEAM_BLUE, 'could be better #teamBlue')
    state.add_message(TEAM_BLUE, 'nice play #teamBlue')
    state.add_message(TEAM_RED, 'not cool #teamRed')
    state.add_message(TEAM_RED, 'oh no! #teamRed')
    state.add_message(TEAM_RED, 'what is going on!? #teamRed')

    print(state.get_summary(TEAM_BLUE))
    print(state.get_summary(TEAM_RED))
