using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public TextMeshProUGUI fpsDisplay;
    public TextMeshProUGUI hpText;
    public PlayerController playerController;
    public Transform environment;

    public GameObject combatUI;
    public GameObject homeUI;
    float fps;

    public event Action OnMapCreated;
    public CombatInput combatInput;
    

    private void Start()
    {
        Application.targetFrameRate = 60;
        InvokeRepeating("GetFps", .1f, 1f);
    }
    void GetFps()
    {
        fps = (int)(1f/Time.smoothDeltaTime);
        fpsDisplay.text = "FPS: " + fps.ToString();
        hpText.text = "HP: " + playerController.hp.ToString();
    }

    public void CreateCombatMap()
    {
        Instantiate(combatInput.map, environment);
        playerController.SetSkin(combatInput.chartacterInfor.model);
        combatUI.SetActive(true);
        homeUI.SetActive(false);


        OnMapCreated?.Invoke();
    }
}
