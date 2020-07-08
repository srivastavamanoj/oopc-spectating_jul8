from transformers import pipeline

PIPELINE_SENTIMENT = 'sentiment-analysis'
PIPELINE_SUMMARIZE = 'summarization'

SUMMARIZER_MODEL = 't5-base'
SUMMARIZER_TOKENIZER = 't5-base'
SUMMARIZER_MIN_LEN = 10
SUMMARIZER_MAX_LEN = 100


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
    state.add_message(TEAM_BLUE, 'This Liverpool team doesnt look like their typical selves! #teamBlue')
    state.add_message(TEAM_BLUE, 'If you dont make liverpool win every game Im going to eat you. #teamBlue')
    state.add_message(TEAM_BLUE, 'Liverpool were caught on the break yet again. Unlike City, Villa didnt have the quality to take advantage of it. #teamBlue')
    state.add_message(TEAM_BLUE, 'Liverpool’s biggest weakness is the counter-attack defense. They are getting counterattacked like crazy, just watch Thursday’s game with Man City and even today. Needs to get fixed asap. #teamBlue')
    state.add_message(TEAM_BLUE, 'Curtis Jones had a blast of a performance. No wonder why Liverpool gave him an contract extention. The future is bright for him. #teamBlue')
    state.add_message(TEAM_BLUE, 'You can tell by our performance against City that the boys mentality has changed a bit. Hopefully this W lights a fire under them. #teamBlue')
    state.add_message(TEAM_BLUE, 'My mans Pepe Reina still playing to this day! Anfield legend. #teamBlue')
    state.add_message(TEAM_BLUE, 'Aston Villa had so many chances, only if they had a solid finisher. #teamBlue')
    state.add_message(TEAM_BLUE, 'Allison is surely the best goalkeeper in the league at the moment. #teamBlue')
    state.add_message(TEAM_BLUE, 'Thank good they were not drunk like against Man City. #teamBlue')
    state.add_message(TEAM_BLUE, 'Missed Reina. Hope he does well. #teamBlue')
    state.add_message(TEAM_BLUE, 'Mo salah he’s a mean man for Liverpool. #teamBlue')
    state.add_message(TEAM_BLUE, 'Bruh Alisson picking that pass out at 7:22 is crazy. #teamBlue')
    state.add_message(TEAM_BLUE, 'IM LITERALLY SO HAPPY LIVERPOOL WON TODAY WHILE MAN CITY LOST WHERES YOUR CONFIDENCE NOW MAN CITY?! #teamBlue')
    state.add_message(TEAM_BLUE, 'I think we could get Mings on the cheap, you know? #teamBlue')
    state.add_message(TEAM_BLUE, 'Salah incredible man! What a assist. #teamBlue')
    state.add_message(TEAM_BLUE, 'Another win, another points. Go Liverpool YNWA. #teamBlue')
    state.add_message(TEAM_BLUE, 'Liverpool been getting exposed at the back on counters, full backs are far too high and its just VVD and Gomez to have to rush back. #teamBlue')
    state.add_message(TEAM_BLUE, 'Virgil been in bad form recently. Looks like hes not focused. #teamBlue')
    state.add_message(TEAM_BLUE, 'They should of give penalty. #teamBlue')
    state.add_message(TEAM_BLUE, 'Ox has his wheels back on.  Well done. #teamBlue')
    state.add_message(TEAM_BLUE, 'I guess the hangover finally wore off. #teamBlue')
    state.add_message(TEAM_BLUE, '11:52 that is terrible defensive position by Trent, he needs to make sure he’s alert on his defensive duties when Gomez switches. #teamBlue')
    state.add_message(TEAM_BLUE, 'Klopp and peps faces after Jones scored, it probably means theyre gonna start him next lol. #teamBlue')
    state.add_message(TEAM_BLUE, 'Mo is dancing all over Villa. Someone please understand how it’s not a foul. If kane does it it’s a pen all day. #teamBlue')
    state.add_message(TEAM_BLUE, 'Man City is losing 1-0 to Southampton lmao. #teamBlue')
    state.add_message(TEAM_BLUE, 'Grealish seems like Aston villas only good player. But he couldn’t finish today. They deserve to be relegated. #teamBlue')
    state.add_message(TEAM_BLUE, 'how is that not a penalty! Wtf?!?. #teamBlue')
    state.add_message(TEAM_BLUE, 'Those subs saved us. #teamBlue')
    state.add_message(TEAM_BLUE, 'AV played a pretty good game against the champs. - Geaux Reds. #teamBlue')
    state.add_message(TEAM_BLUE, 'When you are early and everyone comments first. #teamBlue')
    state.add_message(TEAM_BLUE, 'I know Grealish is the hometown boy but I wish hed go to a big club. #teamBlue')
    state.add_message(TEAM_BLUE, 'This Liverpool team doesnt look like their typical selves! #teamBlue')
    state.add_message(TEAM_BLUE, 'If you dont make liverpool win every game Im going to eat you. #teamBlue')
    state.add_message(TEAM_BLUE, 'alisson had to put a bookmark on his book that he brought. #teamBlue')
    state.add_message(TEAM_BLUE, 'I want to see Minamono play more, who agrees. #teamBlue')
    state.add_message(TEAM_BLUE, 'who else have difficulty differentiate the two team due to their colors?!!! FML. #teamBlue')
    state.add_message(TEAM_BLUE, 'funny how Man City thought they beat Liverpool because they were the better team not because Liverpool dgaf anymore. #teamBlue')
    state.add_message(TEAM_BLUE, 'These were the worst highlights I’ve ever seen. #teamBlue')
    state.add_message(TEAM_BLUE, '6:50 camera man thinkin bout somethin. #teamBlue')
    state.add_message(TEAM_BLUE, 'Naby keita had a game today nice assist. #teamBlue')
    state.add_message(TEAM_BLUE, 'Liverpool need more youth players to spice it up a bit. #teamBlue')
    state.add_message(TEAM_BLUE, 'Mane is simply the best! #teamBlue')
    state.add_message(TEAM_BLUE, 'there were so many counter attacks in this game. #teamBlue')
    state.add_message(TEAM_BLUE, 'This Villa team will give United problems on Thursday. #teamBlue')
    state.add_message(TEAM_BLUE, 'Not sure if Naby is gonna make it, gave up the ball often leading to a counter. #teamBlue')
    state.add_message(TEAM_BLUE, 'Don’t like him paired up with Ox, different team with Gini and Hendo. #teamBlue')
    state.add_message(TEAM_BLUE, 'THE WORLD IN GOD JUDGMENTDAY. #teamBlue')
    state.add_message(TEAM_BLUE, 'Dont worry, Nottingham Forest will sneak into the Premier League, and represent the British Midland region property. #teamBlue')
    state.add_message(TEAM_BLUE, 'Goodbye Aston Villa! Birmingham City will be waiting for you in the Champions League! LOL!!! #teamBlue')

    state.add_message(TEAM_RED, 'Liverpool come on....give the little guys in the Prem a chance. #teamRed')
    state.add_message(TEAM_RED, 'Gini has earned a new contract. #teamRed')
    state.add_message(TEAM_RED, 'Now THATS how you goalkeeping, are you watching ederson. #teamRed')
    state.add_message(TEAM_RED, 'Villa just dont have any bite. #teamRed')
    state.add_message(TEAM_RED, 'Shaq, you deserve better. #teamRed')
    state.add_message(TEAM_RED, 'Meanwhile city lose 1-0 to southapten. #teamRed')
    state.add_message(TEAM_RED, 'I love Liverpool football club. #teamRed')
    state.add_message(TEAM_RED, 'Liverpool got lucky. #teamRed')
    state.add_message(TEAM_RED, 'We need to buy Leicester city number 4.... come on liverpool. #teamRed')
    state.add_message(TEAM_RED, 'Wait the season isn’t over? #teamRed')
    state.add_message(TEAM_RED, 'No one dives like salah or neymar 1:54. #teamRed')
    state.add_message(TEAM_RED, 'What a chaos down there lol. #teamRed')
    state.add_message(TEAM_RED, 'That was a scrappy 70min there. #teamRed')
    state.add_message(TEAM_RED, 'Amazing. #teamRed')
    state.add_message(TEAM_RED, 'Love Liverpool. #teamRed')
    state.add_message(TEAM_RED, 'Subtitles are way off. #teamRed')
    state.add_message(TEAM_RED, 'Jv material. #teamRed')
    state.add_message(TEAM_RED, 'Grealish defensive cover audition tape for man city right here. #teamRed')
    state.add_message(TEAM_RED, 'My reds teams. #teamRed')
    state.add_message(TEAM_RED, 'Aston villa had everything right besides marking. #teamRed')
    state.add_message(TEAM_RED, 'SADIO MANE for PL player of the year. #teamRed')
    state.add_message(TEAM_RED, 'Why do I hear a crowd of no ones there. #teamRed')
    state.add_message(TEAM_RED, 'liverpool dont look like epl champions. Its not often that I am impressed by their performance. City is a better team. #teamRed')
    state.add_message(TEAM_RED, 'What happened minamino ??? #teamRed')
    state.add_message(TEAM_RED, 'Why is he not giving Minamino minutes? #teamRed')
    state.add_message(TEAM_RED, 'Awesome game champs. #teamRed')
    state.add_message(TEAM_RED, 'Second. #teamRed')
    state.add_message(TEAM_RED, 'A Win For Liverpool. #teamRed')
    state.add_message(TEAM_RED, '! #teamRed')
    state.add_message(TEAM_RED, 'if grealish gets injured bye bye villa. #teamRed')
    state.add_message(TEAM_RED, 'Hello big fan. #teamRed')
    state.add_message(TEAM_RED, 'Why could I hear a crowd? #teamRed')
    state.add_message(TEAM_RED, 'How is it that PL games are continuing when Liverpool was named the champions? Someone explain please! #teamRed')
    state.add_message(TEAM_RED, 'Sadio MAIN, the main man for Liverpool! #teamRed')
    state.add_message(TEAM_RED, 'Without the fans, the game is more like a training session. #teamRed')
    state.add_message(TEAM_RED, 'stagnant in the first half but looked a lot better in the second half especially after Hendo came on. #teamRed')
    state.add_message(TEAM_RED, 'Nice to have the best GK in the world also, he made some key saves YNWA. #teamRed')
    state.add_message(TEAM_RED, 'My only question is Between SALAH and JONES who got the curler HAIR. #teamRed')
    state.add_message(TEAM_RED, 'Allison is the best in the world. #teamRed')
    state.add_message(TEAM_RED, 'This was some sloppy soccer. #teamRed')
    state.add_message(TEAM_RED, 'ove how happy Hendo is for Curtis on the goal, even though he couldve hit that himself. What a captain. #teamRed')
    state.add_message(TEAM_RED, 'Liverpool is struggling here. A lot of bad passes. #teamRed')
    state.add_message(TEAM_RED, 'You can just tell how reluctant Salah is to use his right foot, even just to pass. He’s incredible on his left but if it’s all he uses he’s a limited player honestly. #teamRed')
    state.add_message(TEAM_RED, 'Turn off virtual fans. #teamRed')
    state.add_message(TEAM_RED, 'Is mane Muslim? #teamRed')
    state.add_message(TEAM_RED, 'Liverpool is just a tryhard. #teamRed')
    state.add_message(TEAM_RED, 'NBC WILL PRAISE A WHITE CAPTAIN “Henderson” giving him achievement of black player like MANE, GIGI, Alexander, VIRGIL..while they ignored “Kompany” during Mancity days just praising aguero then Kevin De Bruyen. #teamRed')
    state.add_message(TEAM_RED, 'A Win For Liverpool. #teamRed')
    state.add_message(TEAM_RED, 'How does a religiously observant person like Mo Salah square his faith with kneeling before games? #teamRed')
    state.add_message(TEAM_RED, 'this fake fan and stadium noise is terrible. #teamRed')

    print(state.get_summary(TEAM_BLUE))
    print(state.get_summary(TEAM_RED))
