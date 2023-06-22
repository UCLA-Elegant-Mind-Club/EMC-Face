using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using System.IO;
using System;
using TMPro;

public class TextControl : MonoBehaviour
{
    public string[] pos = { "Pos0", "Pos1", "Pos2" };
    public string[] ecc = { "0", "+15", "-15" };
    public string[] texts = { "E", "P", "B" };
    public float wait_time = 1;

    static string dataPath = Directory.GetCurrentDirectory() + "/Assets/Data/";
    string logFile = dataPath + "Data-" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".csv";
    Random rnd = new Random();
    private bool start = false;
    private string log; // new line of data
    private int i, j; // random index
    private bool in_use = false;    // avoid user clicking multiple buttons at same time

    void Start()
    {
        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }
        File.WriteAllText(logFile, "CueShowTime,ObjShowTime,ResponseTime,Eccentricity,Letter,Guess,Correct\n");
    }

    IEnumerator change()
    {
        yield return new WaitForSecondsRealtime(wait_time); // wait before trial starts
        GameObject.Find("Cue").transform.position = GameObject.Find(pos[0]).transform.position; // Cue appears
        log = DateTimeOffset.Now.ToUnixTimeMilliseconds() + ","; // CueShowTime
        yield return new WaitForSecondsRealtime(wait_time); // Cue stays there for this long
        i = rnd.Next(0, pos.Length);
        j = rnd.Next(0, texts.Length);
        GameObject.Find("Cue").transform.position = GameObject.Find("Disappear").transform.position; // Cue disappears
        GameObject.Find(texts[j]).transform.position = GameObject.Find(pos[i]).transform.position; // Letter appears
        log += DateTimeOffset.Now.ToUnixTimeMilliseconds() + ","; // ObjShowTime
        start = true;
        start = true;
        in_use = false;
    }

    void Update()
    {
        string key = "";
        if (!in_use) {
            if (Globals.GetKeyDown(KeyCode.LeftArrow)) { key = "E"; }
            else if (Globals.GetKeyDown(KeyCode.DownArrow)) { key = "P"; }
            else if (Globals.GetKeyDown(KeyCode.RightArrow)) { key = "B"; }
            if (key != "")
            {
                in_use = true;
                if (start)
                {
                    log += DateTimeOffset.Now.ToUnixTimeMilliseconds() + ","; // ResponseTime
                    log += ecc[i] + "," + texts[j] + "," + key + ","; // Eccentricity, Letter, Guess
                    if (texts[j] == key)
                    {
                        log += "True\n";
                    }
                    else
                    {
                        log += "False\n";
                    }
                    File.AppendAllText(logFile, log);
                    log = "";
                }
                for (int k = 0; k < texts.Length; k++)
                {
                    GameObject.Find(texts[k]).transform.position = GameObject.Find("Disappear").transform.position;
                }
                StartCoroutine(change());
            }
        }

        if (Globals.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }
    }
}
