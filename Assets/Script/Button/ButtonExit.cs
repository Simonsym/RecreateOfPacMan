using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ButtonExit : MonoBehaviour
{
    public Button exitButton;
    public GameObject gameCore;

    // Start is called before the first frame update
    void Start()
    {
        exitButton.onClick.AddListener(onExitButtonClick);
    }

    void onExitButtonClick() {
        SceneManager.LoadScene("StartScene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
