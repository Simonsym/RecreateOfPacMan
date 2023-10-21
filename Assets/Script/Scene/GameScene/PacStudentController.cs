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
    public GameObject PacStudent;
    public GameObject gameCore;
    public BoundedStack<Vector2> history;
    public char priv_lastInput = 'd';
    public GameObject sparkPrefab;

    ParticleSystem particleSystem;
    Vector3 movingTarget = Vector3.zero;

    char lastInput = 'd';
    public char currentInput = 'd';

    Vector3 currentPosition = Vector3.zero;

    bool flagPacStudentSetup = false;

    float MOVE_SPEED_MAGNIFICATION = 1.0f;
    Animator pacAnimator;

    GameCore gameCoreScript;
    LevelGenerator levelGeneratorScript;

    List<String> unavailableType = new List<string> {"outside_corner", "outside_wall", "inside_corner", "inside_wall", "t"};

    Dictionary<char, string> turnMode = new Dictionary<char, string> {
        {'w', "ws"}, {'s', "ws"}, {'a', "ad"}, {'d', "ad"},
    };

    bool priv_flagMoving = false;
    bool flagMoving {
        get { return priv_flagMoving; }
        set { particleSystem.GetComponent<Renderer>().enabled = value; priv_flagMoving = value; }
    }

    Dictionary<char, Vector3> nextCellOffset = new Dictionary<char, Vector3> {
        {'w', new Vector3( 0, 1, 0)}, {'s', new Vector3( 0,-1, 0)},
        {'a', new Vector3(-1, 0, 0)}, {'d', new Vector3( 1, 0, 0)}
    };
    
    GameObject square;
    public String tileInfoStr = "";

    AudioSource audioSourceMoving;
    AudioSource audioSourceBrust;
    AudioClip audioMoveEffect;
    AudioClip audioEatBean;
    AudioClip audioDie;
    AudioClip audioCollideOnWall;

    


    // Start is called before the first frame update
    void Start()
    {
        pacAnimator = PacStudent.GetComponent<Animator>();
        

        gameCoreScript = gameCore.GetComponent<GameCore>();
        levelGeneratorScript = gameCore.GetComponent<LevelGenerator>();

        history = new BoundedStack<Vector2>(32);

        square = GameObject.Find("Square");
        square.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);

        var audioSources = GetComponentsInParent<AudioSource>();
        audioSourceMoving = audioSources[0];
        audioSourceBrust = audioSources[1];

        audioSourceMoving = GetComponentInParent<AudioSource>();
        audioMoveEffect = Resources.Load<AudioClip>("EffectSound/moving");
        audioEatBean = Resources.Load<AudioClip>("EffectSound/eat_pellet");
        audioDie = Resources.Load<AudioClip>("EffectSound/death");
        audioCollideOnWall = Resources.Load<AudioClip>("EffectSound/collide_on_wall");

        audioSourceMoving.clip = audioMoveEffect;
        audioSourceMoving.loop = true;
        audioSourceMoving.Play(0);

        audioSourceBrust.clip = audioCollideOnWall;
        audioSourceBrust.loop = false;

        particleSystem = PacStudent.GetComponentInChildren<ParticleSystem>();

    }

    // Update is called once per frame
    void Update()
    {
        // PacStudent.transform.position = new Vector3(-12 * 0.3f + 0.15f, 12 * 0.3f + 0.15f, -1);
        
        if ( Input.GetKey("w")) {  lastInput = 'w'; }
        if ( Input.GetKey("s")) {  lastInput = 's'; }
        if ( Input.GetKey("a")) {  lastInput = 'a'; }
        if ( Input.GetKey("d")) {  lastInput = 'd'; }

        if(movingTarget != Vector3.zero) {
            var boardTarget = worldPosToBoardPos(movingTarget);
            var movingOffset = nextCellOffset[lastInput];
            var targetCell = movingOffset + boardTarget;

            square.transform.position = boardPosToWorldPos(targetCell);

            var tileInfo = levelGeneratorScript.queryTileInfoBoard(targetCell.x, targetCell.y);
            tileInfoStr = tileInfo.ToString();
            
            if(unavailableType.Contains(tileInfo.tileTypeStr)) {
                currentInput = ' ';
            }
            else {
                currentInput = lastInput;
            }

        }


        if(pacAnimator != null) {
            switch(currentInput) {
                case 'w': pacAnimator.Play("Direction Layer.DirectionUp");    pacAnimator.Play("Face Layer.PacFace"); break;
                case 's': pacAnimator.Play("Direction Layer.DirectionDown");  pacAnimator.Play("Face Layer.PacFace"); break;
                case 'a': pacAnimator.Play("Direction Layer.DirectionLeft");  pacAnimator.Play("Face Layer.PacFace"); break;
                case 'd': pacAnimator.Play("Direction Layer.DirectionRight"); pacAnimator.Play("Face Layer.PacFace"); break;
                case ' ':                                                     pacAnimator.Play("Face Layer.PacStop"); break;
                default: break;
            }
        }

        if(currentInput != ' ' && flagMoving) {
            if(!audioSourceMoving.isPlaying) {
                audioSourceMoving.Play(0);
            }
        }
        else {
            audioSourceMoving.Pause();
        }

        if(flagPacStudentSetup) {
            if(!flagMoving) {
                if(currentInput != ' ') {
                    StartCoroutine(MoveStudent(nextCellOffset[currentInput]));
                }
            }
        }


    }

    public void setupCurrentBoardPosition(int x, int y) {
        PacStudent.transform.position = boardPosToWorldPos(new Vector3(x, y, 0));
        flagPacStudentSetup = true;

    }

    public void onMoveEnd() {
        if(currentInput == ' ') {
            audioSourceBrust.clip = audioCollideOnWall;
            audioSourceBrust.Play(0);
            Instantiate(sparkPrefab, PacStudent.transform.position, Quaternion.identity);
        }
    }

    public void onTouchTransmitter(GameObject o) {
        if(o.transform.position.x == -0.5f) {
            PacStudent.transform.position = new Vector3(27.5f, PacStudent.transform.position.y, PacStudent.transform.position.z);
        }
        if(o.transform.position.x == 28.5f) {
            PacStudent.transform.position = new Vector3( 0.5f, PacStudent.transform.position.y, PacStudent.transform.position.z);
        }
    }

    IEnumerator MoveStudent(Vector3 direction) {
        flagMoving = true;

        float moveDuration = 0.5f;
        float elapsedTime = 0f;
        var startPosition = PacStudent.transform.position;
        var targetPosition = startPosition + direction;
        movingTarget = targetPosition;

        while(elapsedTime < moveDuration) {
            PacStudent.transform.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        PacStudent.transform.position = targetPosition;

        flagMoving = false;

        onMoveEnd();


    }


    Vector3 worldPosToBoardPos(Vector3 worldPos) {
        return worldPosToBoardPos(worldPos.x, worldPos.y);
    }

    Vector3 worldPosToBoardPos(float x, float y) {
        return new Vector3(x - 0.50f, y - 0.50f, 0);
    }

    Vector3 boardPosToWorldPos(Vector3 boardPos) {
        return boardPosToWorldPos(boardPos.x, boardPos.y);
    }

    Vector3 boardPosToWorldPos(float x, float y) {
        return getPos(x, y);
    }

    Vector3 getPos(float x, float y) {
        return new Vector3(x + 0.50f, y + 0.50f, 0);
    }

}
