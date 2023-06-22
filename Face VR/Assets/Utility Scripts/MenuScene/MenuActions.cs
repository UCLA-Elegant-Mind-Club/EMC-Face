using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuActions : MonoBehaviour
{
    public static void calibrate()
    {
        Debug.Log("test");
    }
    public static void play()
    {
        SceneManager.LoadScene("Play");
    }
    public static void debug()
    {
        Debug.Log("test");
    }
}
