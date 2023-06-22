from psychopy import iohub, core
import threading, os

import ctypes
lib = ctypes.windll.kernel32

def upTime():
    t = lib.GetTickCount64()
    return t

iohub_config = {'eyetracker.hw.tobii.EyeTracker':
    {'name': 'tracker', 'runtime_settings': {'sampling_rate': 120}}}
io = iohub.launchHubServer(**iohub_config)

# Get the eye tracker device.
tracker = io.devices.tracker

#r = tracker.runSetupProcedure()

def stopEyeTracking():
    tracker.setRecordingState(False)

def __track__(fileName):
    tracker.setRecordingState(True)
    with open(fileName, 'a', newline='') as csvFile:
        writer = csv.writer(csvFile)
        while(tracker.isRecordingEnabled()):
            pos = tracker.getLastGazePosition()
            writer.writerow(["Tobii Stream", uptime(), pos[0], pos[1]])
    csvFile.close()

def startEyeTracking(fileName):
    threading.Thread(target = __track__, args = [fileName]).start()

startEyeTracking(os.path.join(os.getcwd(), 'test.csv'))
core.wait(5)
stopEyeTracking()