import psychopy
from psychopy import gui, visual, core, event, monitors, prefs
import numpy as np  
import os, sys, time, random, math, csv
import string
import threading

date = time.strftime("%m_%d")
expName = {'Experiment Name': ''}
expInfo = {'Subject Name': ''}

dlg = gui.DlgFromDict(dictionary=expName, sortKeys=False, title= "Protocol Name")
if dlg.OK == False:
    core.quit()

dlg = gui.DlgFromDict(dictionary=expInfo, sortKeys=False, title= "Participant Name")
if dlg.OK == False:
    core.quit()
    
# Create folder for each experiment, will contain CSV data file and all eye tracking data files.
# This folder will have the same name as the CSV file.
OUTPATH = os.path.join(os.getcwd(), 'Data', 'Tobii TXTs', \
    (expInfo['Subject Name'] + '_' + date + '_' + expName['Experiment Name']))
if not os.path.isdir(OUTPATH):
    os.mkdir(OUTPATH)

filePath = os.path.join('"' + OUTPATH,\
    (expInfo['Subject Name'] + '_' + date + '_' + expName['Experiment Name'] + ' Eye_Tracking.txt'))

commandLine = 'cmd /k Calibration\TobiiStream\TobiiStream.exe > ' + filePath

def EyeTracking():
    os.system(commandLine)

threading.Thread(target = EyeTracking).start()