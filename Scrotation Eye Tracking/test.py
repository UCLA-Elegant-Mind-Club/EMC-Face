from BG_EyeTracking import *
from psychopy import core
import ctypes
lib = ctypes.windll.kernel32

def upTime():
    t = lib.GetTickCount64()
    return t

print(upTime())
core.wait(5)
print(upTime())

core.quit()