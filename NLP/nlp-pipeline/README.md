Instructions For Setup:
---

1. Install Anaconda and a Python IDE of your choice, PyCharm preferably.

2. Create and activate your Conda environment

`conda env create -f environment.yml`

`conda activate oopc-nlp`

3. Start Flask server - this should start server at localhost:5000

`export FLASK_APP=app.py`

`flask run`

First run would download the models for sentiment analysis and summarizer and hence could take a while to start.
