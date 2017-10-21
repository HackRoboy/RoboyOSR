# RoboyOSR
Offline speech recognition for roboy

# The Idea

Roboys speech recognition does take a very long time to load and send requests to the server.
Therefore the SR does not meet the requirements of a cool DEMO.


# Tooling

## LAN PC
- Lucida for the offline recognition https://github.com/claritylab/lucida
- registered Remote ROS Node for the Roboy PC
- The answer is sent per ROS as Text

## RoboyPC

- Change Speech Recognition Code to stream the Audio to the LAN via GStreamer

# First Steps
- Build and Download Lucida Docker Image
- Modify package that it conects to Roboy as ROS NODE
- Build Streaming Interface on Roboy
- 

# Hardware
- Shuttle PC
- Network Switch
- 2x LAN Cabel
- Monitor