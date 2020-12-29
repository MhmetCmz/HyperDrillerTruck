using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StatusController : MonoBehaviour
{ 
    public int totalDia;
    DrillerMovement drillerMove;
    DrillerCtrl drillerControl;
    private void Start()
    {
        drillerControl = FindObjectOfType<DrillerCtrl>();
        drillerMove = FindObjectOfType<DrillerMovement>();
    }
    public void OnGameBegin()
    { 
        drillerMove.speedLimit = 5;
        drillerControl.WheelAnimsStart();
    }
    public void LevelComplete()
    {
        drillerMove.allowMovement = false;
        FindObjectOfType<UIController>().OnLevelComplete.Invoke(/*StandbyTime*/);
    }
    public void LevelFailed()
    {
        FindObjectOfType<UIController>().OnLevelFailed.Invoke(/*StandbyTime*/); 
    }
}
