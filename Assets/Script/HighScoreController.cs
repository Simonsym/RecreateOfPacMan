using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class HighScoreController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TextMeshProUGUI textMeshProUGUI = GetComponent<TextMeshProUGUI>();

        if(gameObject.name == "u_high_score") {
            textMeshProUGUI.SetText(PlayerPrefs.GetInt("high_score", 0) + "");
        }

        if(gameObject.name == "u_high_score_time") {
            int highScoreTime = PlayerPrefs.GetInt("high_score_time", 0);
            int hours = highScoreTime / 60;
            int second = highScoreTime % 60;
            textMeshProUGUI.SetText(hours.ToString("00") + ":" + second.ToString("00"));
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
