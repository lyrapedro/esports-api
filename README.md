
## Install Python3 Virtual env

    python3 -m venv venv

    source venv/bin/activate

## Install requirements

    pip install -r requirements.txt

    playwright install chromium

## How to use (Local server)
Run the main.py file to run a local api server

    cd src
    PYTHONPATH=.. python main.py

Now you can see the API docs in local server (default: http://localhost:3001/)
    
