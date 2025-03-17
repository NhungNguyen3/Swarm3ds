using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI fpsDisplay;
    float fps;

    private void Start()
    {
        Application.targetFrameRate = 60;
        InvokeRepeating("GetFps", .1f, 1f);
    }
    void GetFps()
    {
        fps = (int)(1f/Time.smoothDeltaTime);
        fpsDisplay.text = "FPS: " + fps.ToString();
    }
}
