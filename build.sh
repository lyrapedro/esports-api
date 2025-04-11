#!/usr/bin/env bash
# build.sh

# Install Python dependencies
pip install -r requirements.txt

# Install Playwright in user mode
pip install playwright

# Install browsers without root
playwright install --with-deps chromium