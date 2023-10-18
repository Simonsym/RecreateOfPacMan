using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

using UnityEditor;
using UnityEditor.Animations;
using TMPro;


public class PacStudentController : MonoBehaviour
{

    char lastInput = 'd';
    char currentInput = 'd';

    Vector3 currentPosition = Vector3.zero;

    /*public char lastInput { 
        get {
            return priv_lastInput; 
        } 
        set {
            // Debug.Log("lastInput::preCheck");
            //if(!canTurn(value)) {
            //    return ;
            //}

            //currentInput = value;
            //Debug.Log("lastInput::postCheck");
            //if(pacAnimator != null) {
                switch(value) {
           //         case 'w': pacAnimator.Play("Direction Layer.DirectionUp"); break;
           //         case 's': pacAnimator.Play("Direction Layer.DirectionDown"); break;
           //         case 'a': pacAnimator.Play("Direction Layer.DirectionLeft"); break;
            //        case 'd': pacAnimator.Play("Direction Layer.DirectionRight"); break;
                    default: break;
                }
            }

            //priv_lastInput = value;

        }
    }*/


   
    public BoundedStack<Vector2> history;
    public char priv_lastInput = 'd';

    public float currentX = 1;
    public float currentY = 1;

    float MOVE_SPEED_MAGNIFICATION = 1.0f;
    Animator pacAnimator;
    public GameObject PacStudent;
    
    public GameObject gameCore;
    GameCore gameCoreScript;

    Dictionary<char, string> turnMode = new Dictionary<char, string> {
        {'w', "ws"}, {'s', "ws"}, {'a', "ad"}, {'d', "ad"},
    };

    bool flagMoving = false;
    public float targetX = 1;
    public float targetY = 1;

    Dictionary<char, Vector3> nextCellOffset = new Dictionary<char, Vector3> {
        {'w', new Vector3( 0, 1, 0)}, {'s', new Vector3( 0,-1, 0)},
        {'a', new Vector3(-1, 0, 0)}, {'d', new Vector3( 1, 0, 0)}
    };
    

    // Start is called before the first frame update
    void Start()
    {
        pacAnimator = PacStudent.GetComponent<Animator>();

        gameCoreScript = gameCore.GetComponent<GameCore>();

        history = new BoundedStack<Vector2>(32);

    }

    // Update is called once per frame
    void Update()
    {
        // PacStudent.transform.position = new Vector3(-12 * 0.3f + 0.15f, 12 * 0.3f + 0.15f, -1);
        
        if ( Input.GetKey("w")) {  lastInput = 'w'; }
        if ( Input.GetKey("s")) {  lastInput = 's'; }
        if ( Input.GetKey("a")) {  lastInput = 'a'; }
        if ( Input.GetKey("d")) {  lastInput = 'd'; }

        if(checkWall() && checkInCell(0.1f)) {
            currentInput = lastInput;
        }

        /*
        var currentPos = worldPosToBoardPos(PacStudent.transform.position);
        currentX = currentPos.x;
        currentY = currentPos.y;

        if(superimposeCheck(worldPosToBoardPos(PacStudent.transform.position), new Vector3(targetX, targetY, 0))) {
            setToCell();
        }

        if(checkReachTheDestination()) {
            flagMoving = setNextCell();
        }
        else {
            flagMoving = true;
        }

        // Debug.Log($"currentX: {currentX} currentY: {currentY} targetX: {targetX} targetY: {targetY}");
    */
    }

    bool checkWall() {
        var offset = nextCellOffset[lastInput];

        Vector3 currentBoardPosition = worldPosToBoardPos(PacStudent.transform.position);

        Vector3 targetBoardPosition = new Vector3((float)Math.Round(currentBoardPosition.x), (float)Math.Round(currentBoardPosition.y), 0);

        targetBoardPosition += offset;

        // {"empty", "outside_corner", "outside_wall", "inside_corner", "inside_wall", "normal_pellet", "power_pellet", "t"};
        var nextPosWall = gameCoreScript.getExtendedLevelMap()[(int)targetBoardPosition.x, (int)targetBoardPosition.y];
        var isWallList = new bool[8]{false, true, true, true, true, false, false, true};
        
        return isWallList[nextPosWall];
    }

    bool checkInCell(float accurate) {
        Vector3 currentBoardPosition = worldPosToBoardPos(PacStudent.transform.position);

        Vector3 targetBoardPosition = new Vector3((float)Math.Round(currentBoardPosition.x), (float)Math.Round(currentBoardPosition.y), 0);

        var Distance = Vector3.Distance(currentBoardPosition, targetBoardPosition);

        // Debug.Log("currentBoardPosition: " + currentBoardPosition + " targetBoardPosition: " + targetBoardPosition);

        return Distance < accurate;
    }



    void FixedUpdate() {

        if(currentX == targetX && currentY == targetY) {
            // return ;
        }

        if(!flagMoving) {
            // return ;
        }

        Vector3 offset = nextCellOffset[currentInput] * (MOVE_SPEED_MAGNIFICATION * Time.deltaTime);
        
        var currentPos = PacStudent.transform.position;

        PacStudent.transform.position = new Vector3(currentPos.x + offset.x, currentPos.y + offset.y, 0);

    }

    public void setupCurrentBoardPosition(int x, int y) {
        PacStudent.transform.position = boardPosToWorldPos(new Vector3(x, y, 0));
        
        /*
        currentX = x;
        currentY = y;
        targetX = x;
        targetY = y;

        history.Push(new Vector2(currentX, currentY));

        PacStudent.transform.position = boardPosToWorldPos(currentX, currentY);
        */
    }

    public void movePacStudent(Vector3 boardPosition) {

    }

/*
    public bool setNextCell() {
        var nextOffset = nextCellOffset[lastInput];

        var nextX = (int)(currentX + nextOffset.x);
        var nextY = (int)(currentY + nextOffset.y);

        int [,] levelMap = gameCoreScript.getLevelMap();
        int height = levelMap.GetLength(0) * 2;
        int width = levelMap.GetLength(1) * 2;

        Debug.Log("nextX: " + nextX + " nextY: " + nextY);

        if(nextX < -width || nextY < -height || width < nextX || height < nextY) {
            return false;
        }
        else {
            targetX = nextX;
            targetY = nextY;
            
            return true;
        }

    }


    public bool checkReachTheDestination() {
        return currentX == targetX && currentY == targetY;
    }

    public bool superimposeCheck(Vector3 a, Vector3 b) {
        if((Vector2.Distance(a, b)  < 0.05f)) {
            Debug.Log("superimposeCheck::Distance: " + (Vector2.Distance(a, b)  < 0.05f));
        }
        
        return Vector2.Distance(a, b) < 0.1f;
    }

    public void setToCell() {
        setupCurrentBoardPosition((int)targetX, (int)targetY);
    }


    public bool inCellCheck(float accurate) {
        Vector3 currentBoardPosition = worldPosToBoardPos(PacStudent.transform.position);

        Vector3 targetBoardPosition = new Vector3((float)Math.Round(currentBoardPosition.x), (float)Math.Round(currentBoardPosition.y), 0);

        var Distance = Vector3.Distance(currentBoardPosition, targetBoardPosition);


        Debug.Log("currentBoardPosition: " + currentBoardPosition + " targetBoardPosition: " + targetBoardPosition);

        return Distance < 0.10f;
    }

    public bool canTurn(char newDirection) {

        bool flagInSameAxis = turnMode[newDirection].Contains(priv_lastInput);

        bool flagInCell = inCellCheck(0.1f);

        return flagInSameAxis || flagInCell;
    }

    */



    Vector3 worldPosToBoardPos(Vector3 worldPos) {
        return worldPosToBoardPos(worldPos.x, worldPos.y);
    }

    Vector3 worldPosToBoardPos(float x, float y) {
        return new Vector3((x - 0.15f) / 0.3f, (y - 0.15f) / 0.3f, 0);
    }

    Vector3 boardPosToWorldPos(Vector3 boardPos) {
        return boardPosToWorldPos(boardPos.x, boardPos.y);
    }

    Vector3 boardPosToWorldPos(float x, float y) {
        return getPos(x, y);
    }

    Vector3 getPos(float x, float y) {
        return new Vector3(x * 0.3f + 0.15f, y * 0.3f + 0.15f, 0);
    }

}
