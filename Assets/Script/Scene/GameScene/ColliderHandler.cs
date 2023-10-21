using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ColliderHandler : MonoBehaviour
{
    PacStudentController pacStudentController;
    GameCore gameCore;


    // Start is called before the first frame update
    void Start()
    {
        pacStudentController = GetComponentInParent<PacStudentController>();
        gameCore = GameObject.Find("GameCore").GetComponent<GameCore>();
        Debug.Log(gameCore);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        
        //Debug.Log($"OnTriggerEnter2D::{other.gameObject.name} uid: {other.gameObject.GetInstanceID()}");
        

        switch(other.gameObject.name) {
            case "normal_pellet":
            case "bonus_score_cherry":
            case "power_pellet": {
                gameCore.onEatPellet(other.gameObject);
                break ;
            }
            case "transmitter_square": {
                pacStudentController.onTouchTransmitter(other.gameObject);
                break ;
            }
            case "GhostSprite": {
                gameCore.onTouchGhost(other.gameObject);
                break;
            }

        }


    }

}
