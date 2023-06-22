from psychopy import gui, core, prefs
from psychopy.sound import Sound
prefs.hardware['audioLib'] = ['ptb', 'pyo']
import os, time, random, math
from ScrotrackingTVStimuli import TVStimuli

##### Parent Rotation Class #####
##### Parent Rotation Class #####
##### Parent Rotation Class #####

class RotationProtocol (TVStimuli):
    rotations = [0, 30, 60, 90, 120, 150, 180, -150, -120, -90, -60, -30]
    
    def initRotations(self, rotations):
        self.rotations = rotations
    
    def instructions(self):
        self.genDisplay('Welcome player. In this module, there will be 1 set of 3 ' + self.stimDescription + self.stimType + 's', 0, 6)
        self.genDisplay('that you will have to memorize to 3 different keys. After a short training and', 0, 3)
        self.genDisplay('practice round, your mission will be to recognize these ' + self.stimType + 's as fast as', 0, 0)
        self.genDisplay('possible when they have been rotated, so make sure to use your dominant hand!', 0, -3)
        self.genDisplay('Press spacebar to continue.', 0, -6)
        self.showWait()
        self.genDisplay('The faster you respond, the more points you can score - you can win up to 1000', 0, 8)
        self.genDisplay('points in each trial. However, after ' + str(self.timeOut) + ' seconds, you\'ll automatically lose', 0, 5)
        self.genDisplay('400 points for taking too long. If you make an error, you\'ll also lose points, but', 0, 2)
        self.genDisplay('slightly less than 400. However, try not to randomly guess. Your trial number will', 0, -1)
        self.genDisplay('only advance for correct trials, so you\'ll have the same chances to win points.', 0, -4)
        self.genDisplay('Press spacebar to continue.', 0, -7)
        self.showWait()
        self.demo()
        self.showHighScores()
    
    def showImage(self, set, showTarget, rotation):
        self.displayImage.image = self.getImage(set, showTarget)
        self.displayImage.ori = rotation
        self.displayImage.draw()
    
    def getImage(self, set, showTarget):
        pass;
    
    def initFile(self):
        self.csvOutput(["Correct Response","Rotation (deg)", "Reaction Time (ms)", "Target", "CPU Uptime", "UTC time"])
        
    def demoSequence(self, rotations, demoMessage):
        self.genDisplay(demoMessage, 0, 8)
        self.showImage(self.numSets, 0, self.refValue)
        self.genDisplay('(Press space to rotate)', 0, -8)
        self.showWait()
        for rotation in rotations:
            self.genDisplay(demoMessage, 0, 8)
            self.showImage(self.numSets, 0, rotation)
            self.showWait(0.1)
        self.genDisplay(demoMessage, 0, 8)
        self.showImage(self.numSets, 0, self.refValue)
        self.genDisplay('(Press space to continue)', 0, -8)
        self.showWait()
    
    def demo(self):
        self.demoSequence(self.rotations, 'The ' + self.stimType + 's will be rotated in a circle as shown below.')
    
    def csvOutput(self, output):
        super().csvOutput(output);
        if(output[1] == 180):
            output[1] = -180
            super().csvOutput(output);

##### Parent Scaling Class #####
##### Parent Scaling Class #####
##### Parent Scaling Class #####
class ScalingProtocol(TVStimuli):
    sizes = [1, 2, 4, 8, 16, 28]
    
    def initSizes(self, sizes):
        for i in range(0, len(self.sizes) - 1):
            diff = math.log10(self.sizes[i + 1] / self.sizes[i])/2
            intermed = self.sizes[i] * 10 ** diff
            sizes = sizes + [round(intermed,2)]
        sizes.sort()
        self.sizes = sizes
        self.refValue = self.referenceSize
    
    def instructions(self):
        self.genDisplay('Welcome player. In this module, there will be 1 set of 3 ' + self.stimDescription + self.stimType + 's', 0, 6)
        self.genDisplay('that you will have to memorize to 3 different keys. After a short training and', 0, 3)
        self.genDisplay('practice round, your mission will be to recognize these ' + self.stimType + 's as fast as', 0, 0)
        self.genDisplay('possible when they have been rescaled, so make sure to use your dominant hand!', 0, -3)
        self.genDisplay('Press spacebar to continue.', 0, -6)
        self.showWait()
        self.genDisplay('The faster you respond, the more points you can score - you can win up to 1000', 0, 8)
        self.genDisplay('points in each trial. However, after ' + str(self.timeOut) + ' seconds, you\'ll automatically lose', 0, 5)
        self.genDisplay('400 points for taking too long. If you make an error, you\'ll also lose points, but', 0, 2)
        self.genDisplay('slightly less than 400. However, try not to randomly guess. Your trial number will', 0, -1)
        self.genDisplay('only advance for correct trials, so you\'ll have the same chances to win points.', 0, -4)
        self.genDisplay('Press spacebar to continue.', 0, -7)
        self.showWait()
        self.demo()
        self.showHighScores()
    
    def initFile(self):
        self.csvOutput(["Correct Response","Height (deg)", "Reaction Time (ms)", "Target", "CPU Uptime", "UTC time"])
    
    def showImage(self, set, showTarget, size):
        self.displayImage.image = self.getImage(set, showTarget)
        faceWidth = self.angleCalc(size) * float(self.tvInfo['faceWidth'])
        faceHeight = self.angleCalc(size) * float(self.tvInfo['faceHeight'])
        self.displayImage.size = (faceWidth, faceHeight)
        self.displayImage.draw()
    
    def getImage(self, set, showTarget):
        pass;
    
    def demoSequence(self, sizes, demoMessage):
        self.genDisplay(demoMessage, 0, 8)
        self.showImage(self.numSets, 0, self.referenceSize)
        self.genDisplay('(Press space to rescale)', 0, -8)
        self.showWait()
        for size in sizes:
            self.genDisplay(demoMessage, 0, 8)
            self.showImage(self.numSets, 0, size)
            self.showWait(0.1)
        self.genDisplay(demoMessage, 0, 8)
        self.showImage(self.numSets, 0, self.referenceSize)
        self.genDisplay('(Press space to continue)', 0, -8)
        self.showWait()
        
    def demo(self):
        self.demoSequence(self.sizes, 'The ' + self.stimType + 's will be rescaled as shown below.')

###Famous Faces###
###Famous Faces###
###Famous Faces###
class FamousFaces (TVStimuli):
    names = ["Biden", "Putin", "Trump", "Oprah Winfrey"]

    def __init__(self, testValues, fileName = ''):
        super().__init__(testValues, 'Famous', 'Face', fileName = fileName)
    
    def getImage(self, set, showTarget):
        fileName = self.names[set * 3 + showTarget] + '.png'
        return os.path.join(os.getcwd(), 'Stimuli', 'Famous Faces', fileName)

#Rotation
class FamousFacesRoll(FamousFaces, RotationProtocol):
    
    def __init__(self, fileName = ''):
        self.initRotations(self.rotations)
        super().__init__(self.rotations, fileName = fileName)

#Scaling
class FamousFacesScaling(FamousFaces, ScalingProtocol):
    
    def __init__(self, fileName = ''):
        self.initSizes(self.sizes)
        super().__init__(self.sizes, fileName = fileName)

###Trained Faces###
###Trained Faces###
###Trained Faces###
class TrainedFaces (FamousFaces):
    names = ["Virginia", "Brenda", "Nicole", "Vicky", "Beth", "Naomi", "Velma", "Brittany", "Natalie"]

    def __init__(self, testValues, fileName = ''):
        TVStimuli.__init__(self, testValues, 'Familiar', 'Face', fileName = fileName)
    
    def getImage(self, fileName):
        return os.path.join(os.getcwd(), 'Stimuli', 'Unfamiliar Faces', fileName)

#Rotation
class TrainedFacesRoll(TrainedFaces, RotationProtocol):
    def __init__(self, fileName = ''):
        self.initRotations(self.rotations)
        super().__init__(self.rotations, fileName = fileName)
    
    def getImage(self, set, showTarget):
        targets = [[1,2,3],[1,2,3],['demo']]
        fileName = 'face ' + str(targets[set][showTarget]) + '.png'
        return super().getImage(os.path.join('Roll', fileName))

class TrainedFacesYaw(TrainedFaces, RotationProtocol):
    rotations = list(range(-90, 105, 15))
    def __init__(self, fileName = ''):
        self.initRotations(self.rotations)
        super().__init__(self.rotations, fileName = fileName)
    
    def showImage(self, set, showTarget, rotation):
        self.displayImage.image = self.getImage(set, showTarget, rotation)
        self.displayImage.draw()
    
    def getImage(self, set, showTarget, rotation):
        targets = [[1,2,3],[1,2,3],['demo']]
        folderName = 'Face ' + str(targets[set][showTarget])
        return super().getImage(os.path.join('Yaw', folderName, str(rotation) + '.png'))
    
    def demo(self):
        self.demoSequence(self.rotations, 'The faces will be rotated left and right as shown below.')

class TrainedFacesPitch(TrainedFaces, RotationProtocol):
    rotations = [-60, -52.5, -45, -37.5, -30, -22.5, -15, -7.5, 0, 7.5, 15, 22.5, 30, 37.5, 45, 52.5, 60]
    def __init__(self, fileName = ''):
        self.initRotations(self.rotations)
        super().__init__(self.rotations, fileName = fileName)
    
    def showImage(self, set, showTarget, rotation):
        self.displayImage.image = self.getImage(set, showTarget, rotation)
        self.displayImage.draw()
    
    def getImage(self, set, showTarget, rotation):
        targets = [[1,2,3],[1,2,3],['demo']]
        folderName = 'Face ' + str(targets[set][showTarget])
        return super().getImage(os.path.join('Pitch', folderName, str(rotation) + '.png'))
    
    def demo(self):
        self.demoSequence(self.rotations, 'The faces will be rotated up and down as shown below.')

#Scaling
class TrainedFacesScaling(TrainedFaces, ScalingProtocol):
    
    def __init__(self, fileName = ''):
        self.initSizes(self.sizes)
        super().__init__(self.sizes, fileName = fileName)
    
    def getImage(self, set, showTarget):
        targets = [[1,2,3],[1,2,3],['demo']]
        fileName = 'face ' + str(targets[set][showTarget]) + '.png'
        return super().getImage(os.path.join('Roll', fileName))

###Unfamiliar Faces###
###Unfamiliar Faces###
###Unfamiliar Faces###
class UnfamiliarFaces (TVStimuli):

    def __init__(self, testValues, fileName = ''):
        TVStimuli.__init__(self, testValues, 'Unfamiliar', 'Face', fileName = fileName)
    
    def getImage(self, fileName):
        return os.path.join(os.getcwd(), 'Stimuli', 'Unfamiliar Faces', fileName)

#Rotation
class UnfamiliarFacesRoll(UnfamiliarFaces, RotationProtocol):
    def __init__(self, fileName = ''):
        self.initRotations(self.rotations)
        super().__init__(self.rotations, fileName = fileName)
    
    def getImage(self, set, showTarget):
        targets = [[1,2,3],[1,2,3],['demo']]
        fileName = 'face ' + str(targets[set][showTarget]) + '.png'
        return super().getImage(os.path.join('Roll', fileName))

class UnfamiliarFacesYaw(UnfamiliarFaces, RotationProtocol):
    rotations = list(range(-90, 105, 15))
    def __init__(self, fileName = ''):
        self.initRotations(self.rotations)
        super().__init__(self.rotations, fileName = fileName)
    
    def showImage(self, set, showTarget, rotation):
        self.displayImage.image = self.getImage(set, showTarget, rotation)
        self.displayImage.draw()
    
    def getImage(self, set, showTarget, rotation):
        targets = [[1,2,3],[1,2,3],['demo']]
        folderName = 'Face ' + str(targets[set][showTarget])
        return super().getImage(os.path.join('Yaw', folderName, str(rotation) + '.png'))
    
    def demo(self):
        self.demoSequence(self.rotations, 'The faces will be rotated left and right as shown below.')

class UnfamiliarFacesPitch(UnfamiliarFaces, RotationProtocol):
    rotations = [-60, -52.5, -45, -37.5, -30, -22.5, -15, -7.5, 0, 7.5, 15, 22.5, 30, 37.5, 45, 52.5, 60]
    def __init__(self, fileName = ''):
        self.initRotations(self.rotations)
        super().__init__(self.rotations, fileName = fileName)
    
    def showImage(self, set, showTarget, rotation):
        self.displayImage.image = self.getImage(set, showTarget, rotation)
        self.displayImage.draw()
    
    def getImage(self, set, showTarget, rotation):
        targets = [[1,2,3],[1,2,3],['demo']]
        folderName = 'Face ' + str(targets[set][showTarget])
        return super().getImage(os.path.join('Pitch', folderName, str(rotation) + '.png'))
    
    def demo(self):
        self.demoSequence(self.rotations, 'The faces will be rotated up and down as shown below.')

#Scaling
class UnfamiliarFacesScaling(UnfamiliarFaces, ScalingProtocol):
    
    def __init__(self, fileName = ''):
        self.initSizes(self.sizes)
        super().__init__(self.sizes, fileName = fileName)
    
    def getImage(self, set, showTarget):
        targets = [[1,2,3],[1,2,3],['demo']]
        fileName = 'face ' + str(targets[set][showTarget]) + '.png'
        return super().getImage(os.path.join('Roll', fileName))

###Scalicity Covert###
###Scalicity Covert###
###Scalicity Covert###         this is size=6deg, ecc=+/-40deg, NO EYETRACKING
class ScalicityCovert (TVStimuli):
    sizes = [2, 4, 8]
    angles = [-30, -15, 0, 15, 30]
    refValue = (TVStimuli.referenceSize, 0)
    
    def __init__(self, stimDescription, stimType, fileName = ''):
        values = []
        for size in self.sizes:
            for ang in self.angles:
                values += [(size, ang)]
        super().__init__(values, stimDescription, stimType, fileName)
    
    def instructions(self):
        self.genDisplay('Welcome player. In this module, there will be 1 set of 3 ' + self.stimDescription + self.stimType + 's', 0, 6)
        self.genDisplay('that you will have to memorize to 3 different keys. After a short training and', 0, 3)
        self.genDisplay('practice round, your mission will be to recognize these ' + self.stimType + 's as fast as possible', 0, 0)
        self.genDisplay('when they have been moved, so make sure to use your dominant hand!', 0, -3)
        self.genDisplay('Press spacebar to continue.', 0, -6)
        self.showWait()
        self.genDisplay('The faster you respond, the more points you can score - you can win up to 1000', 0, 8)
        self.genDisplay('points in each trial. However, after ' + str(self.timeOut) + ' seconds, you\'ll automatically lose', 0, 5)
        self.genDisplay('400 points for taking too long. If you make an error, you\'ll also lose points, but', 0, 2)
        self.genDisplay('slightly less than 400. However, try not to randomly guess. Your trial number will', 0, -1)
        self.genDisplay('only advance for correct trials, so you\'ll have the same chances to win points.', 0, -4)
        self.genDisplay('Press spacebar to continue.', 0, -7)
        self.showWait()
        self.genDisplay('This is a covert protocol!', 0, 6)
        self.genDisplay('During trials, keep your eyes on the center of the screen,', 0, 3)
        self.genDisplay('and only use your peripheral vision to see non-centered images.', 0, 0)
        self.genDisplay('Press spacebar to continue.', 0, -3)
        self.showWait()
        self.demo()
        self.showHighScores()
        self.genDisplay('Are you ready?', 0, 3, height = 3)
        self.genDisplay('Press space to start.', 0, -2)
        self.showWait()
    
    def initFile(self):
        self.csvOutput(["Correct Response", ("Size (deg)", "Eccentricity (deg)"), "Reaction Time (ms)", "Target", "CPU Uptime", "UTC time"])

    def csvOutput(self, output):
        testVal = output[1]
        if type(testVal) == int or type(testVal) == float: output[1] = (testVal, -1)
        super().csvOutput([output[0]] + [output[1][0], output[1][1]] + output[2:])
    
    def demoSequence(self, testVars, demoMessage, buttonMessage, showTime):
        self.genDisplay(demoMessage, 0, 8)
        self.showImage(self.numSets, 0, self.refValue)
        self.genDisplay('(Press space to ' + buttonMessage + ')', 0, -8)
        self.showWait()
        for testVar in testVars:
            self.genDisplay(demoMessage, 0, 8)
            self.showImage(self.numSets, 0, testVar)
            self.showWait(showTime)
        self.genDisplay(demoMessage, 0, 8)
        self.showImage(self.numSets, 0, self.refValue)
        self.genDisplay('(Press space to continue)', 0, -8)
        self.showWait()
        
    def demo(self):
        self.demoSequence([(self.referenceSize, ang) for ang in self.angles], 'The stimuli will be moved as shown below.', 'move', 0.1)
    
class UnfamiliarFacesCovert (ScalicityCovert):
    def __init__(self, fileName = ''):
        super().__init__('Unfamiliar', 'Face', fileName)
    
    def showImage(self, set, showTarget, testVar):
        targets = [[7,8,9], [7,8,9], ['demo']];
        fileName = 'face ' + str(targets[set][showTarget]) + '.png'
        self.displayImage.image = os.path.join(os.getcwd(), 'Stimuli', 'Unfamiliar Faces', 'Roll', fileName)
        faceWidth = self.angleCalc(testVar[0]) * float(self.tvInfo['faceWidth'])
        faceHeight = self.angleCalc(testVar[0]) * float(self.tvInfo['faceHeight'])
        pos_cm = math.tan(testVar[1] * (math.pi/180)) * float(self.tvInfo['Distance to screen'])
        self.displayImage.size = (faceWidth, faceHeight)
        self.displayImage.pos = (pos_cm, 0)
        self.displayImage.draw()

class EPBCovert (ScalicityCovert):
    def __init__(self, fileName = ''):
        super().__init__('English', 'Character', fileName)
    
    def showImage(self, set, showTarget, testVar):
        targets = [[1,2,3], [1,2,3], ['demo']];
        fileName = 'char ' + str(targets[set][showTarget]) + '.png'
        self.displayImage.image = os.path.join(os.getcwd(), 'Stimuli', 'English Characters', fileName)
        faceWidth = self.angleCalc(testVar[0]) * float(self.tvInfo['faceWidth'])
        faceHeight = self.angleCalc(testVar[0]) * float(self.tvInfo['faceHeight'])
        pos_cm = math.tan(testVar[1] * (math.pi/180)) * float(self.tvInfo['Distance to screen'])
        self.displayImage.size = (faceWidth, faceHeight)
        self.displayImage.pos = (pos_cm, 0)
        self.displayImage.draw()

###Scalicity Overt###
###Scalicity Overt###
###Scalicity Overt###         this is size=6deg, ecc=+/-40deg, NO EYETRACKING
class ScalicityOVERT (TVStimuli):
    sizes = [2, 4, 8]
    angles = [-30, -15, 0, 15, 30]
    refValue = (TVStimuli.referenceSize, 0)
    
    def __init__(self, stimDescription, stimType, fileName = ''):
        values = []
        for size in self.sizes:
            for ang in self.angles:
                values += [(size, ang)]
        super().__init__(values, stimDescription, stimType, fileName)
    
    def instructions(self):
        self.genDisplay('Welcome player. In this module, there will be 1 set of 3 ' + self.stimDescription + self.stimType + 's', 0, 6)
        self.genDisplay('that you will have to memorize to 3 different keys. After a short training and', 0, 3)
        self.genDisplay('practice round, your mission will be to recognize these ' + self.stimType + 's as fast as possible', 0, 0)
        self.genDisplay('when they have been moved and rescaled, so make sure to use your dominant hand!', 0, -3)
        self.genDisplay('Press spacebar to continue.', 0, -6)
        self.showWait()
        self.genDisplay('The faster you respond, the more points you can score - you can win up to 1000', 0, 8)
        self.genDisplay('points in each trial. However, after ' + str(self.timeOut) + ' seconds, you\'ll automatically lose', 0, 5)
        self.genDisplay('400 points for taking too long. If you make an error, you\'ll also lose points, but', 0, 2)
        self.genDisplay('slightly less than 400. However, try not to randomly guess. Your trial number will', 0, -1)
        self.genDisplay('only advance for correct trials, so you\'ll have the same chances to win points.', 0, -4)
        self.genDisplay('Press spacebar to continue.', 0, -7)
        self.showWait()
        self.genDisplay('This is an OVERT protocol!', 0, 6)
        self.genDisplay('You may move your eyes freely,', 0, 3)
        self.genDisplay('and do NOT have to only use peripheral vision.', 0, 0)
        self.genDisplay('Press spacebar to continue.', 0, -3)
        self.showWait()
        self.demo()
        self.showHighScores()
        self.genDisplay('Are you ready?', 0, 3, height = 3)
        self.genDisplay('Press space to start.', 0, -2)
        self.showWait()
    
    def initFile(self):
        self.csvOutput(["Correct Response", ("Size (deg)", "Eccentricity (deg)"), "Reaction Time (ms)", "Target", "CPU Uptime", "UTC time"])

    def csvOutput(self, output):
        testVal = output[1]
        if type(testVal) == int or type(testVal) == float: output[1] = (testVal, -1)
        super().csvOutput([output[0]] + [output[1][0], output[1][1]] + output[2:])
    
    def demoSequence(self, testVars, demoMessage, buttonMessage, showTime):
        self.genDisplay(demoMessage, 0, 8)
        self.showImage(self.numSets, 0, self.refValue)
        self.genDisplay('(Press space to ' + buttonMessage + ')', 0, -8)
        self.showWait()
        for testVar in testVars:
            self.genDisplay(demoMessage, 0, 8)
            self.showImage(self.numSets, 0, testVar)
            self.showWait(showTime)
        self.genDisplay(demoMessage, 0, 8)
        self.showImage(self.numSets, 0, self.refValue)
        self.genDisplay('(Press space to continue)', 0, -8)
        self.showWait()
        
    def demo(self):
        self.demoSequence([(self.referenceSize, ang) for ang in self.angles], 'The stimuli will be moved as shown below.', 'move', 0.1)

class UnfamiliarFacesOVERT (ScalicityOVERT):
    def __init__(self, fileName = ''):
        super().__init__('Unfamiliar', 'Face', fileName)
    
    def showImage(self, set, showTarget, testVar):
        targets = [[7,8,9], [7,8,9], ['demo']];
        fileName = 'face ' + str(targets[set][showTarget]) + '.png'
        self.displayImage.image = os.path.join(os.getcwd(), 'Stimuli', 'Unfamiliar Faces', 'Roll', fileName)
        faceWidth = self.angleCalc(testVar[0]) * float(self.tvInfo['faceWidth'])
        faceHeight = self.angleCalc(testVar[0]) * float(self.tvInfo['faceHeight'])
        pos_cm = math.tan(testVar[1] * (math.pi/180)) * float(self.tvInfo['Distance to screen'])
        self.displayImage.size = (faceWidth, faceHeight)
        self.displayImage.pos = (pos_cm, 0)
        self.displayImage.draw()

class EPBOVERT (ScalicityOVERT):
    def __init__(self, fileName = ''):
        super().__init__('English', 'Character', fileName)
    
    def showImage(self, set, showTarget, testVar):
        targets = [[1,2,3], [1,2,3], ['demo']];
        fileName = 'char ' + str(targets[set][showTarget]) + '.png'
        self.displayImage.image = os.path.join(os.getcwd(), 'Stimuli', 'English Characters', fileName)
        faceWidth = self.angleCalc(testVar[0]) * float(self.tvInfo['faceWidth'])
        faceHeight = self.angleCalc(testVar[0]) * float(self.tvInfo['faceHeight'])
        pos_cm = math.tan(testVar[1] * (math.pi/180)) * float(self.tvInfo['Distance to screen'])
        self.displayImage.size = (faceWidth, faceHeight)
        self.displayImage.pos = (pos_cm, 0)
        self.displayImage.draw()