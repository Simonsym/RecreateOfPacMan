using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Diagnostics;


public class GameUI : MonoBehaviour
{

    DateTime ghostScareCountDownStartTime;
    DateTime ghostScareCountDownEndTime;

    bool flagGhostScareEnabled = false;
    public GameObject ghostScareLabel;
    public GameObject ghostScareText;
    public GameObject scoreText;
    public GameObject timeText;

    public Button exitButton;
    public RawImage heart_0;
    public RawImage heart_1;
    public RawImage heart_2;
    public GameObject gameCore;
    public GameCore gameCoreScript;

    void showHideScare(bool _switch) {
        ghostScareLabel.SetActive(_switch);
        ghostScareText.SetActive(_switch);
    }

    void setupScare(int duration) {
        ghostScareCountDownStartTime = DateTime.Now;
        ghostScareCountDownEndTime = ghostScareCountDownStartTime + new TimeSpan(0, 0, duration);
        showHideScare(true);
        flagGhostScareEnabled = true;

    }


    // Start is called before the first frame update
    void Start()
    {
        gameCoreScript = gameCore.GetComponent<GameCore>();
        setupScare(20);

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(scoreText.GetType().ToString());
        scoreText.GetComponent<TextMeshProUGUI>().SetText(gameCoreScript.score.ToString());
        timeText.GetComponent<TextMeshProUGUI>().SetText((DateTime.Now - gameCoreScript.startTime).ToString().Substring(3, 10));
        if(flagGhostScareEnabled) {
            ghostScareText.GetComponent<TextMeshProUGUI>().SetText(((int)(ghostScareCountDownEndTime - DateTime.Now).TotalSeconds).ToString());
            if((ghostScareCountDownEndTime - DateTime.Now).TotalSeconds <= 0) {
                flagGhostScareEnabled = false;
                showHideScare(false);
            }
        }
        
    }
}
