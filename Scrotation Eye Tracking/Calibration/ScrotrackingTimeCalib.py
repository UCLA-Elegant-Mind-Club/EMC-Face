from psychopy import gui, core, prefs
from psychopy.sound import Sound
prefs.hardware['audioLib'] = ['ptb', 'pyo']
import os, time, csv, re, uuid
from ScrotrackingTVStimuli import TVStimuli as TV
from ScrotrackingTVStimuli import upTime
from BG_EyeTracking import *

monitorFile = 'ScrotrackingMonitors.csv'

def calibrate():
    #standard calibration uses Knudson TV data
    #if taking data remotely, replace the calibration file with your own before running code
    label = TV.calibrate(os.path.join(os.getcwd(), 'Calibration', monitorFile))
    if label != None:
        return " " + label
    else:
        calibDlg = gui.Dlg(title='Calibration File Required',
            labelButtonOK=' I have a file ready. Open File Chooser. ', labelButtonCancel=' I have not calibrated my system. Cancel Experiment. ', screen=-1)
        calibDlg.addText('Could not find system calibration. You will now be prompted to choose your calibration file.')
        calibDlg.addText('If you have not yet recieved a calibration csv, you can run the script in the \'Calibration\' folder.\n')
        calibDlg.show()
        if not calibDlg.OK: core.quit()
        
        calibFile = gui.fileOpenDlg(os.getcwd(), prompt = "Select Calibration File", allowed = '*.csv')
        if calibFile == None: core.quit()
        with open(os.path.join(os.getcwd(), 'Calibration', calibFile[0])) as file:
            tvInfo = list(csv.reader(file, delimiter=','))
        
        calibDlg = gui.Dlg(title='System Description', screen=-1)
        calibDlg.addText('Please provide a short description of the system like your name and type of computer.')
        calibDlg.addField('Description: ')
        description = calibDlg.show()
        if description == None: core.quit()
        macAddress = ':'.join(re.findall('..', '%012x' % uuid.getnode()))
        with open(os.path.join(os.getcwd(), 'Calibration', monitorFile)) as file:
            rows = sum(1 for row in file if row[0:3] == 'Rem')
        with open(os.path.join(os.getcwd(), 'Calibration', monitorFile), 'a', newline='') as file:
            writer = csv.writer(file)
            writer.writerow(["Remote " + str(rows + 1), description[0], macAddress] + tvInfo[1])
        return " " + TV.calibrate(os.path.join(os.getcwd(), 'Calibration', monitorFile))

def main():
    # Prepare directories
    monNum = calibrate()
    for i in range(0, 1000):
        TV.genDisplay(upTime(), 0, 0, height = 2)
        TV.win.flip()

main()