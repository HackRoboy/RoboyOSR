#!/bin/bash

gst-launch alsasrc device=hw:2 ! audioconvert ! audioresample  ! alawenc ! rtppcmapay ! udpsink host=192.168.20.26 port=5001
