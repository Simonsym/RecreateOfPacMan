using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

using UnityEditor;
using UnityEditor.Animations;


public class PacStudentController : MonoBehaviour
{

    char priv_currentDirection = 'R';
    char lastDirection = 'R';
    public float currentX = 1;
    public float currentY = 1;

    float MOVE_SPEED_MAGNIFICATION = 4.2f;

    Animator pacAnimator;
    public GameObject PacStudent;



    // Start is called before the first frame update
    void Start()
    {
        pacAnimator = PacStudent.GetComponent<Animator>();
        currentDirection = ' ';

    }

    // Update is called once per frame
    void Update()
    {
        // PacStudent.transform.position = new Vector3(-12 * 0.3f + 0.15f, 12 * 0.3f + 0.15f, -1);
        // var cellPosition = coordinateReverseMapping(currentX, currentY);

        if ( Input.GetKey("up")) {  currentDirection = 'U';  }
        if ( Input.GetKey("down")) {  currentDirection = 'D';  }
        if ( Input.GetKey("left")) {  currentDirection = 'L';  }
        if ( Input.GetKey("right")) {  currentDirection = 'R';  }

    }

    void FixedUpdate() {
        switch(currentDirection) {
            case 'L':
                currentX -= (float)(MOVE_SPEED_MAGNIFICATION * Time.deltaTime);
                break;
            case 'R':
                currentX += (float)(MOVE_SPEED_MAGNIFICATION * Time.deltaTime);
                break;
            case 'U':
                currentY += (float)(MOVE_SPEED_MAGNIFICATION * Time.deltaTime);
                break;
            case 'D':
                currentY -= (float)(MOVE_SPEED_MAGNIFICATION * Time.deltaTime);
                break;
            default:
                break;
        }
        
        var newPos = getPos(currentX, currentY);
        PacStudent.transform.position = new Vector3(newPos.x, newPos.y, 0);

    }


    public char currentDirection { 
        get {
            return priv_currentDirection; 
        } 
        set {
            if(pacAnimator != null) {
                switch(value) {
                    case 'U': pacAnimator.Play("Direction Layer.DirectionUp"); break;
                    case 'D': pacAnimator.Play("Direction Layer.DirectionDown"); break;
                    case 'L': pacAnimator.Play("Direction Layer.DirectionLeft"); break;
                    case 'R': pacAnimator.Play("Direction Layer.DirectionRight"); break;
                    default: break;
                }
            }

            priv_currentDirection = value;

        }
    }

    int[,] getLevelMap() {
        int[,] levelMap = {
            {1,2,2,2,2,2,2,2,2,2,2,2,2,7},
            {2,5,5,5,5,5,5,5,5,5,5,5,5,4},
            {2,5,3,4,4,3,5,3,4,4,4,3,5,4},
            {2,6,4,0,0,4,5,4,0,0,0,4,5,4},
            {2,5,3,4,4,3,5,3,4,4,4,3,5,3},
            {2,5,5,5,5,5,5,5,5,5,5,5,5,5},
            {2,5,3,4,4,3,5,3,3,5,3,4,4,4},
            {2,5,3,4,4,3,5,4,4,5,3,4,4,3},
            {2,5,5,5,5,5,5,4,4,5,5,5,5,4},
            {1,2,2,2,2,1,5,4,3,4,4,3,0,4},
            {0,0,0,0,0,2,5,4,3,4,4,3,0,3},
            {0,0,0,0,0,2,5,4,4,0,0,0,0,0},
            {0,0,0,0,0,2,5,4,4,0,3,4,4,0},
            {2,2,2,2,2,1,5,3,3,0,4,0,0,0},
            {0,0,0,0,0,0,5,0,0,0,4,0,0,0},
        };

        return levelMap;
    }


    Vector2Int coordinateReverseMapping(float x, float y) {
        int ix = (int)Math.Floor(x);
        int iy = (int)Math.Floor(y);

        return new Vector2Int(ix + 14, 14 - iy);
    }

    Vector3 getPos(float x, float y) {
        return new Vector3(x * 0.3f + 0.15f, y * 0.3f + 0.15f, -1);
    }


}
