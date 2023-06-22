from __future__ import absolute_import, division
import psychopy
psychopy.useVersion('latest')
from psychopy import locale_setup, prefs, sound, gui, visual, core, data, event, logging, clock, monitors
import numpy as np
from numpy import (sin, cos, tan, log, log10, pi, average,
                   sqrt, std, deg2rad, rad2deg, linspace, asarray)
from numpy.random import random, randint, normal, shuffle
from psychopy.hardware import keyboard
import os, time, csv, random
from BG_EyeTracking import *

angles = [0, 7, 14, 21]
sizes = [4, 6 ,8]
directions = [0, 2] #0 is right, 2 is left
letters = list('EPB')
trials = 5

def csvOutput(output, fileName):
    with open(fileName, 'a', newline='') as csvFile:
        writer = csv.writer(csvFile)
        writer.writerow(output)
    csvFile.close()
    
def csvInput(fileName):
    with open(fileName) as csvFile:
        reader = csv.DictReader(csvFile, delimiter = ',')
        dict = next(reader)
    csvFile.close()
    return dict

_thisDir = os.path.dirname(os.path.abspath(__file__))
os.chdir(_thisDir)
gaborfile = os.path.join(os.getcwd(), '55TV-C (Computer4).csv')
if not os.path.isfile(gaborfile):
    print('You must run the eccentricity_calibration.py script to set up your monitor')
    time.sleep(5)
    core.quit()

tvInfo = csvInput(gaborfile)

distToScreen = float(tvInfo['Distance to screen'])
heightMult, spacer = float(tvInfo['height']), float(tvInfo['spacer'])
circleMult = float(tvInfo['circleRadius'])
centerX, centerY = float(tvInfo['centerx']), float(tvInfo['centery'])
rightXMult, leftXMult = float(tvInfo['rightx']), float(tvInfo['leftx'])
rightEdge, leftEdge = float(tvInfo['rightEdge']), float(tvInfo['leftEdge'])

def endExp():
    win.flip()
    logging.flush()
    win.close()
    core.quit()

datadlg = gui.Dlg(title='Record Data?', pos=None, size=None, style=None,\
     labelButtonOK=' Yes ', labelButtonCancel=' No ', screen=-1)
ok_data = datadlg.show()
recordData = datadlg.OK

if recordData:
    date = time.strftime("%m_%d")
    expName = 'EPB Covert and 3 Sizes'
    expInfo = {'Subject Name': ''}
    
    dlg = gui.DlgFromDict(dictionary=expInfo, sortKeys=False, title=expName)
    if dlg.OK == False:
        core.quit()
    
    OUTPATH = os.path.join(os.getcwd(), 'Data')
    if not os.path.isdir(OUTPATH):
        os.mkdir(OUTPATH)
    
    fileName = os.path.join(OUTPATH,\
        (expInfo['Subject Name'] + '_' + date + '_' + expName + '.csv'))
        
datadlg = gui.Dlg(title='Select cross position', screen=-1)
datadlg.addField('Position: ', choices = ["Center", "Right", "Left"])
ok_data2 = datadlg.show()
if ok_data2 is None:
    endExp()
elif ok_data2[0] == 'Left':
    centerX = -(leftEdge-3)
    dirExclusions = [2]
elif ok_data2[0] == 'Right':
    centerX = rightEdge-3
    dirExclusions = [0]
else:
    dirExclusions = []

headers = ['Letter', 'Direction', 'Eccentricity', 'Reaction Time (s)', 'Stim Size']
if not os.path.isfile(fileName):
    csvOutput(headers, fileName)

mon = monitors.Monitor('TV') # Change this to the name of your display monitor
mon.setWidth(float(tvInfo['Width (cm)']))
win = visual.Window(
    size=(int(tvInfo['Width (px)']), int(tvInfo['Height (px)'])), fullscr=True, screen=int(tvInfo['Screen number']), 
    winType='pyglet', allowGUI=True, allowStencil=False,
    monitor= mon, color='grey', colorSpace='rgb',
    blendMode='avg', useFBO=True, 
    units='cm')
    
def genDisplay(displayInfo):
    displayText = visual.TextStim(win=win,
    text= displayInfo['text'],
    font='Arial',
    pos=(displayInfo['xPos'], displayInfo['yPos']),
    height=displayInfo['heightCm'],
    wrapWidth=500,
    ori=0, 
    color=displayInfo['color'],
    colorSpace='rgb',
    opacity=1, 
    languageStyle='LTR',
    depth=0.0)
    return displayText
    
def displaceCalc(angle):
    angleRad = np.deg2rad(angle)
    xDisp = np.tan(angleRad)*distToScreen
    return xDisp
    
def checkcorrect(response, letter):
    if response == 'escape':
        endExp()
    elif response == 'v':
        ans = 'E'
    elif response == 'b':
        ans = 'P'
    elif response == 'n':
        ans = "B"
    return (ans == letter)

cross = visual.ShapeStim(
    win=win, name='Cross', vertices='cross',units='cm', 
    size=(1, 1),
    ori=0, pos=(centerX, centerY),
    lineWidth=1, lineColor=[1,1,1], lineColorSpace='rgb',
    fillColor=[1,1,1], fillColorSpace='rgb',
    opacity=1, depth=-1.0, interpolate=True)
    
radius = displaceCalc(4)*circleMult


def instructions():
    genDisplay({'text': 'Press "V" when you see the "E" displayed',\
        'xPos': 0, 'yPos': centerY+4, 'heightCm': 1, 'color': 'white'}).draw()
    genDisplay({'text': 'and "B" when you see the "P" displayed',\
        'xPos': 0, 'yPos': centerY+2, 'heightCm': 1, 'color': 'white'}).draw()
    genDisplay({'text': 'and "N" when you see the "B" displayed',\
        'xPos': 0, 'yPos': centerY,'heightCm': 1, 'color': 'white'}).draw()
    genDisplay({'text': 'Please keep your eyes fixed in the center',\
        'xPos': 0, 'yPos': centerY-2,'heightCm': 1, 'color': 'white'}).draw()
    genDisplay({'text': 'Press the spacebar to continue',\
        'xPos': 0, 'yPos': centerY-4,'heightCm': 1, 'color': 'white'}).draw()
    win.flip()
    keyy = event.waitKeys(keyList = ['space', 'escape']) 
    if keyy[0] == 'escape': 
        win.flip()
        logging.flush()
        win.close()
        core.quit()

def expBreak():
    dispInfo = {'text': 'Break', 'xPos': 0, 'yPos': centerY+4, 'heightCm': 3, 'color': 'white'}
    breakText = genDisplay(dispInfo)
    dispInfo = {'text': '', 'xPos': 0, 'yPos': centerY, 'heightCm': 3, 'color': 'white'}
    for i in range(20):
        breakText.draw()
        dispInfo['text'] = str(20-i) + ' seconds'
        genDisplay(dispInfo).draw()
        win.flip()
        time.sleep(1)
        
        
def inBounds(trialInfo):
    if trialInfo['dir'] in dirExclusions:
        return False
    if trialInfo['dir'] == 0:
        if (centerX + displaceCalc(trialInfo['angle'])) > rightEdge:
            return False
    elif trialInfo['dir'] == 2:
        if (centerX - displaceCalc(trialInfo['angle'])) < (-leftEdge):
            return False
    return True

def genPairs():
    pairs = list(range(0))
    for i in range(trials):
        for j in range(len(angles)):
            for k in range(len(directions)):
                for l in range(len(sizes)):
                    pairs.append((j*10)+k+(l*.1))
    random.shuffle(pairs)
    return pairs
    
def interpretPair(pair):
    angle = angles[int(pair/10)]
    direction = directions[int(pair%10)]
    stimsize = sizes[round((pair-int(pair))/0.1)]
    letter = random.choice(letters)
    return {'angle': angle, 'dir': direction, 'letter': letter, 'size': stimsize}
       
    
    
    
instructions()

pairs = genPairs()



#correct = 0
#incorrect = 1

run = 0
mistakes = 0


mistakedict = {}

for pair in pairs:
    win.flip()
    
    trialInfo = interpretPair(pair)
    if not inBounds(trialInfo):
        continue
    cross.draw()
    time.sleep(.1)
    win.flip()
    interstimulus = random.uniform(.3,.8)
    time.sleep(interstimulus)
    displacement = displaceCalc(trialInfo['angle'])
    if trialInfo['dir'] == 0:
        xPos = centerX + displacement*rightXMult
    elif trialInfo['dir'] ==2:
        xPos = centerX + displacement*leftXMult
    
    stimheight = displaceCalc(trialInfo['size'])*heightMult
    displayInfo = {'text': trialInfo['letter'], 'xPos': xPos, 'yPos': centerY, 'heightCm': stimheight, 'color': 'white'}
    displayText = genDisplay(displayInfo)
    displayText.draw()
    times = {'start': 0, 'end': 0}
    win.timeOnFlip(times, 'start')
    win.flip()
    keys = event.waitKeys(timeStamped = True, keyList = ['v', 'b', 'n', 'escape'])
    key = keys[0]
    if key[0] == 'escape':
        endExp()
    times['end'] = key[1]
    reactionTime = times['end'] - times['start']
    buffer = 2.3 - interstimulus - reactionTime
    if buffer > 0:
        if checkcorrect(key[0], trialInfo['letter']):
            output = (trialInfo['letter'], trialInfo['dir'], trialInfo['angle'], reactionTime, trialInfo['size'])
            csvOutput(output, fileName)
        else:
            mistakedict[mistakes] = trialInfo
            mistakes += 1
            output = (trialInfo['letter'], trialInfo['dir'], trialInfo['angle'], 0, trialInfo['size'])
            csvOutput(output, fileName)
    else:
        mistakedict[mistakes] = trialInfo
        mistakes += 1
        output = (trialInfo['letter'], trialInfo['dir'], trialInfo['angle'], 0, trialInfo['size'])
        csvOutput(output, fileName)
    run += 1
    win.flip()
    if run%52 == 0 and run != 208:
        expBreak()
    if buffer > 0:
        time.sleep(buffer)

run2 = 0
if mistakes > 0:
    genDisplay({'text': 'These trials are a make-up of your mistakes',\
        'xPos': 0, 'yPos': centerY+5, 'heightCm': 1, 'color': 'white'}).draw()
    genDisplay({'text': 'Please follow the same instructions',\
        'xPos': 0, 'yPos': centerY+3, 'heightCm': 1, 'color': 'white'}).draw()
    genDisplay({'text': 'and press Space to continue',\
        'xPos': 0, 'yPos': centerY+1, 'heightCm': 1, 'color': 'white'}).draw()
    win.flip()
    keyyy = event.waitKeys(keyList = ['space', 'escape'])
    if keyyy[0] == 'escape': 
        win.flip()
        logging.flush()
        win.close()
        core.quit()
    l = 0
    while l < mistakes:
        win.flip()
        trialInfo = mistakedict[l]
        if not inBounds(trialInfo):
            continue
        cross.draw()
        time.sleep(.1)
        win.flip()
        interstimulus2 = random.uniform(.3,.8)
        time.sleep(interstimulus2)
        displacement = displaceCalc(trialInfo['angle'])
        if trialInfo['dir'] == 0:
            xPos = centerX + displacement*rightXMult
        elif trialInfo['dir'] ==2:
            xPos = centerX + displacement*leftXMult  
        
        stimheight = displaceCalc(trialInfo['size'])*heightMult
        displayInfo = {'text': trialInfo['letter'], 'xPos': xPos, 'yPos': centerY, 'heightCm': stimheight, 'color': 'white'}
        displayText = genDisplay(displayInfo)
        displayText.draw()
        times = {'start': 0, 'end': 0}
        win.timeOnFlip(times, 'start')
        win.flip()
        keys = event.waitKeys(timeStamped = True, keyList = ['v', 'b', 'n', 'escape'])
        key2 = keys[0]
        if key2[0] == 'escape':
            endExp()
        times['end'] = key2[1]
        reactionTime = times['end'] - times['start']
        buffer2 = 2.3 - interstimulus2 - reactionTime
        if buffer2 > 0:
            if checkcorrect(key2[0], trialInfo['letter']):
                output = (trialInfo['letter'], trialInfo['dir'], trialInfo['angle'], reactionTime, trialInfo['size'])
                csvOutput(output, fileName)
            else:
                mistakedict[mistakes] = trialInfo
                mistakes += 1
                output = (trialInfo['letter'], trialInfo['dir'], trialInfo['angle'], 0, trialInfo['size'])
                csvOutput(output, fileName)
        else:
            mistakedict[mistakes] = trialInfo
            mistakes += 1
            output = (trialInfo['letter'], trialInfo['dir'], trialInfo['angle'], 0, trialInfo['size'])
            csvOutput(output, fileName)
        run2 += 1
        l += 1
        win.flip()
        if len (dirExclusions) == 0 and run2%52 == 0:
            expBreak()
        if buffer2 > 0:
            time.sleep(buffer2)
        
strmistakes = str(mistakes)
print(strmistakes + ' mistakes')
endExp()


