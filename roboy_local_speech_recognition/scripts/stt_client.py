#!/usr/bin/env python

import sys
import rospy
import ipdb

import os.path, sys
sys.path.append(os.path.join(os.path.dirname(os.path.realpath(__file__)), os.pardir))

from roboy_communication_cognition.srv import RecognizeSpeech

def stt_client():
	rospy.wait_for_service("RecognizeSpeech")
	try:
		stt = rospy.ServiceProxy('RecognizeSpeech', RecognizeSpeech)
		resp = stt()
		print resp.text
		return resp.text
	except rospy.ServiceException, e:
		print "Service call failed: %s"%e

if __name__ == "__main__":
	stt_client()