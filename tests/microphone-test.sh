#!/bin/bash
arecord -f S16_LE -r 16000 | python client.py -u ws://10.177.254.60:8080/client/ws/speech -r 32000 -

#arecord -D hw:0,0 -f S16_LE -r 16000 | python client.py -u ws://10.177.254.60:8080/client/ws/speech -r 32000 -

