using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Diagnostics;


public class GameUI : MonoBehaviour
{

    float ghostScareCountDownStartTime;
    float ghostScareCountDownEndTime;

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

    GameObject ui_count_down; 

    public void showHideScare(bool _switch) {
        ghostScareLabel.GetComponent<TextMeshProUGUI>().enabled = _switch;
        ghostScareText.GetComponent<TextMeshProUGUI>().enabled = _switch;

    }

    public void setScare(int duration) {
        ghostScareCountDownStartTime = Time.time;
        ghostScareCountDownEndTime = ghostScareCountDownStartTime + duration + 0.3f;
        showHideScare(true);
        flagGhostScareEnabled = true;

    }


    // Start is called before the first frame update
    void Start()
    {
        gameCoreScript = gameCore.GetComponent<GameCore>();
        ui_count_down = GameObject.Find("u_count_down");
        
        StartCoroutine(countDown());

    }

    // Update is called once per frame
    void Update()
    {
        scoreText.GetComponent<TextMeshProUGUI>().SetText(gameCoreScript.score.ToString());
        timeText.GetComponent<TextMeshProUGUI>().SetText(new TimeSpan(0, 0, (int)Math.Round(Time.time - gameCoreScript.startTime)).ToString());

        if(flagGhostScareEnabled) {
            ghostScareText.GetComponent<TextMeshProUGUI>().SetText(((int)(ghostScareCountDownEndTime - Time.time)).ToString());
            if((ghostScareCountDownEndTime - Time.time) <= 0) {
                flagGhostScareEnabled = false;
                showHideScare(false);
            }
        }
    }


    IEnumerator countDown() {
        bool flagFunctionEnd = false;
        while(!flagFunctionEnd) {
            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(1);
            ui_count_down.GetComponent<TextMeshProUGUI>().SetText("2");
            yield return new WaitForSecondsRealtime(1);
            ui_count_down.GetComponent<TextMeshProUGUI>().SetText("1");
            yield return new WaitForSecondsRealtime(1);
            ui_count_down.GetComponent<TextMeshProUGUI>().SetText("Go!");
            Time.timeScale = 1;
            flagFunctionEnd = true;
            ui_count_down.GetComponent<TextMeshProUGUI>().enabled = false;
            break;
        }
    }

}
