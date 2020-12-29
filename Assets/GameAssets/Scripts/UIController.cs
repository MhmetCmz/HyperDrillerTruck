using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public UnityEvent OnLevelComplete;
    public UnityEvent OnLevelFailed;
    public UnityEvent TutorialBegin;
    public UnityEvent CloseSwipeDown;
    public UnityEvent OpenSwipeUp;
    public UnityEvent TutorialEnd;
    public Text lvlText,diaText;
    public int currentLevel, totalDia, playedBefore; 
    // Start is called before the first frame update
    void Start()
    {
        currentLevel = PlayerPrefs.GetInt("level") + 1;
        lvlText.text = "level " + currentLevel;
        totalDia = PlayerPrefs.GetInt("diamond");
        diaText.text = totalDia.ToString();
        playedBefore = PlayerPrefs.GetInt("firstPlay", 0);
    }

    public void updateDiaText()
    {
        diaText.text = totalDia.ToString();
    }

    public void UpdateDiaPref()
    {
        PlayerPrefs.SetInt("diamond", totalDia);
    } 
} 
