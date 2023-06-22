using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varjo.XR;


/*

public methods:
CalibrateGaze() -- calibrates using set mode
StartLog() -- start logging eye tracking data
EndLog() -- finish logging eye tracking data

may want to call VarjoEyeTracking.IsGaze(Allowed|Available|Calibrated)() before tracking and issue warning to console else
we can get the XR rig with "this"

NOTE: this script does *not* track eye measurements (interpupillary distance, pupil dilation, etc.)
 - we can add this functionality if desired!

*/

// Attach to anything.
public class EyeTracking_Nik : MonoBehaviour {
    [Header("Main camera (under XR Rig)")]
    public Camera xrCamera;

    [Header("Logging toggle key")]
    public KeyCode loggingToggleKey = KeyCode.RightControl;

    [Header("Gaze calibration settings")]
    [Tooltip("Legacy - 10 dots without priors; Fast: 5 dots; One Dot: quickest, least accurate")]
    public VarjoEyeTracking.GazeCalibrationMode gazeCalibrationMode = VarjoEyeTracking.GazeCalibrationMode.Fast;
    [Tooltip("Keyboard shortcut to request calibration")]
    public KeyCode calibrationKey = KeyCode.Space;

    [Header("Gaze output filter")]
    [Tooltip("Standard: smoothing on gaze data; None: raw data")]
    public VarjoEyeTracking.GazeOutputFilterType gazeFilterType = VarjoEyeTracking.GazeOutputFilterType.None;

    
    private VarjoEyeTracking.GazeOutputFrequency frequency = VarjoEyeTracking.GazeOutputFrequency.MaximumSupported;
    private List<VarjoEyeTracking.GazeData> dataSinceLastUpdate;
    private StreamWriter writer = null;
    private bool logging = false;

    private static readonly string[] Columns = { "CaptureTime", "HeadsetPos", "HeadsetRotation", "CombinedGazeForward", 
        "FocusDistance", "FocusStability", "CalcXEccentricity", "CalcYEccentricity", "LeftForward", "RightForward", "LeftPosition", "RightPosition" };
    private static string VectorPrecision = "F6"; // preciison after decimal for vector printouts
    private static string DegreePrecision = "F1"; // precision after decimal for calculated eccentricities in degrees


    // Start is called before the first frame update
    void Start() {
        VarjoEyeTracking.SetGazeOutputFrequency(frequency);
    }

    // Update is called once per frame
    public void Update() {
        if (Input.GetKeyDown(calibrationKey))
            CalibrateGaze();

        if (Input.GetKeyDown(loggingToggleKey)) {
            if (!logging)
                StartLogging();
            else
                StopLogging();
            
            return;
        }

        if (logging) {
            int dataCount = VarjoEyeTracking.GetGazeList(out dataSinceLastUpdate);

            for (int i = 0; i < dataCount; ++i)
                LogGazeData(dataSinceLastUpdate[i]);
        }
    }

    public void CalibrateGaze() {
        VarjoEyeTracking.RequestGazeCalibration(gazeCalibrationMode);
    }

    public void StartLogging() {
        if (logging) {
            Debug.LogWarning("StartLogging was called when already logging. No new log started.");
            return;
        }

        logging = true;

        string logDir = Globals.dataPath;
        Directory.CreateDirectory(logDir);

        DateTime now = DateTime.Now;
        string fileName = string.Format("ET-{0}-{1:00}-{2:00}-{3:00}-{4:00}", now.Year, now.Month, now.Day, now.Hour, now.Minute);

        string logPath = logDir + fileName + ".csv";
        writer = new StreamWriter(logPath);

        Log(Columns);
        Debug.Log("Log file started at " + logPath);
    }

    public void StopLogging() {
        if (!logging)
            return;

        if (writer != null) {
            writer.Flush();
            writer.Close();
            writer = null;
        }

        logging = false;
        Debug.Log("Logging ended.");
    }

    void LogGazeData(VarjoEyeTracking.GazeData data) {
        // if data isn't valid, don't log
        if (data.status == VarjoEyeTracking.GazeStatus.Invalid)
            return;

        string[] logData = new string[Columns.Length];

        // capture time (Unix ms timestamp)
        logData[0] = ((DateTimeOffset) VarjoTime.ConvertVarjoTimestampToDateTime(data.captureTime)).ToUnixTimeMilliseconds().ToString();

        // headset position + rotation
        logData[1] = xrCamera.transform.localPosition.ToString(VectorPrecision);
        logData[2] = xrCamera.transform.localRotation.ToString(VectorPrecision);

        // combined gaze forward
        logData[3] = data.gaze.forward.ToString(VectorPrecision);

        // focus distance
        logData[4] = data.focusDistance.ToString();
        logData[5] = data.focusStability.ToString();

        // calculated X, Y eccentricities
        float x = data.gaze.forward.x;
        float y = data.gaze.forward.y;
        float z = data.gaze.forward.z;
        logData[6] = ( Math.Atan(x / z) / Math.PI * 180 ).ToString(DegreePrecision);
        logData[7] = ( Math.Atan(y / z) / Math.PI * 180 ).ToString(DegreePrecision);

        // left and right eye forward
        bool leftInvalid = data.leftStatus == VarjoEyeTracking.GazeEyeStatus.Invalid;
        logData[8] = leftInvalid ? "" : data.left.forward.ToString(VectorPrecision);

        bool rightInvalid = data.rightStatus == VarjoEyeTracking.GazeEyeStatus.Invalid;
        logData[9] = rightInvalid ? "" : data.right.forward.ToString(VectorPrecision);

        // left and right eye position
        logData[10] = leftInvalid ? "" : data.left.origin.ToString("F3");
        logData[11] = leftInvalid ? "" : data.right.origin.ToString("F3");

        Log(logData);
        
        
    }

    void Log(string[] values) {
        if (!logging || writer == null)
            return;

        string line = "";
        for (int i = 0; i < values.Length; ++i) {
            values[i] = values[i].Replace("\r", "").Replace("\n", "");
            line += values[i] + (i == values.Length - 1 ? "" : ";");
        }

        writer.WriteLine(line);
        writer.Flush();
    }

    void OnApplicationQuit() {
        StopLogging();
    }
}
