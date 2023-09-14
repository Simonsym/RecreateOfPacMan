using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

using UnityEditor;
using UnityEditor.Animations;

public class GameCore : MonoBehaviour
{

    static readonly string GAME_BACKGROUND = "Sound/game_background";
    AudioSource backgroundPlayer;
    Tilemap gameTileMap;
    Grid gameGrid;
    AudioClip backgroundSound;
    Vector2Int ghostPosition = new Vector2Int(0, 0);

    public GameObject PowerPelletPrefab;
    public GameObject GhostPrefab;
    public GameObject NormalPelletPrefab;


    void PlayIntroSound() {
        backgroundSound = Resources.Load<AudioClip>(GAME_BACKGROUND);

        backgroundPlayer = GetComponent<AudioSource>();
        backgroundPlayer.Play(0);
        Invoke("StopIntroSound", 5);
    }

    void StopIntroSound() {
        backgroundPlayer.Stop();

        Debug.Log("Will play background: " + backgroundSound.name);
        backgroundPlayer.clip = backgroundSound;
        backgroundPlayer.loop = true;
        backgroundPlayer.Play(0);

    }

    char priv_currentDirection = 'R';
    char lastDirection = 'R';

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
    public float currentX = 1;
    public float currentY = 1;

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

    Vector2Int coordinateMapping(int x, int y) {
        return new Vector2Int(x - 14, 14 - y);
    }

    Vector2Int coordinateReverseMapping(float x, float y) {
        int ix = (int)Math.Floor(x);
        int iy = (int)Math.Floor(y);

        return new Vector2Int(ix + 14, 14 - iy);
    }

    GameObject PacStudent;

    void putPacStudent() {
        List<int> availableType = new List<int>() {0, 5, 6};
        
        int availableX = -1;
        int availableY = -1;
        
        for(int r = 0; r < 15; r++) {
            for(int c = 0; c < 14; c++) {
                // Debug.Log("" + r + ", " + c + ", " + getLevelMap()[r, c]);
                if(availableType.Contains(getLevelMap()[r, c])) {
                    availableX = c;
                    availableY = r;
                    break ;
                }
            }
            if(availableX != -1) {
                break ;
            }
        }

        if (availableX == -1) {
            Debug.Log("Not find available tile");
            return ;
        }

        // var convertedCoordinate = coordinateMapping(availableX, availableY);

        PacStudent = GameObject.Find("go_pac_student");
        var availablePair = coordinateMapping(availableX, availableY);

        currentX = availablePair.x;
        currentY = availablePair.y;

    }

    Vector3 getPos(float x, float y) {
        return new Vector3(x * 0.3f + 0.15f, y * 0.3f + 0.15f, -1);
    }

    public Animator pacAnimator;

    void delegateOfILevelGenerator(string type, Vector2 position) {
        switch(type) {
            case "normal_pellet": {
                Instantiate(NormalPelletPrefab, new Vector3(position.x, position.y, -1), Quaternion.identity);
                break;
            }
            case "power_pellet": {
                Instantiate(PowerPelletPrefab, new Vector3(position.x, position.y, -1), Quaternion.identity);
                break;
            }
            default: {
                Debug.Log("[+] " + type + " - " + position);
                break;
            }
        }


    }

    // Start is called before the first frame update
    void Start()
    {
        gameTileMap = GameObject.Find("GameTilemap").GetComponent<Tilemap>();
        gameGrid = GameObject.Find("GameGrid").GetComponent<Grid>();

        PlayIntroSound();
        putPacStudent();

        currentDirection = ' ';

        pacAnimator = PacStudent.GetComponent<Animator>();

        if(Config.ENABLE_NEW_LEVEL_GENERATOR) {

        }
        else {
            Invoke("ClassicLevelGenerator_instance_GeneratorMap", 0.1f);
        }


    }

    void ClassicLevelGenerator_instance_GeneratorMap() {
        ClassicLevelGenerator.instance.GeneratorMap(delegateOfILevelGenerator);
    }

    void runCircle() {
        var cellPosition = coordinateReverseMapping(currentX, currentY);

        if(cellPosition.x == 6 && cellPosition.y == 1) {
            currentDirection = 'D';
        }
        if(cellPosition.x == 6 && cellPosition.y == 6) {
            currentDirection = 'L';
        }
        if(cellPosition.x == 0 && cellPosition.y == 6) {
            currentDirection = 'U';
        }
        if(cellPosition.x == 0 && cellPosition.y == 1) {
            currentDirection = 'R';
        }
    }

    // Update is called once per frame
    void Update()
    {
        // PacStudent.transform.position = new Vector3(-12 * 0.3f + 0.15f, 12 * 0.3f + 0.15f, -1);
        var cellPosition = coordinateReverseMapping(currentX, currentY);


    }

    float PLAYER_MOVE_SPEED = 0.07f;

    void FixedUpdate() {
        switch(currentDirection) {
            case 'L':
                currentX -= PLAYER_MOVE_SPEED;
                break;
            case 'R':
                currentX += PLAYER_MOVE_SPEED;
                break;
            case 'U':
                currentY += PLAYER_MOVE_SPEED;
                break;
            case 'D':
                currentY -= PLAYER_MOVE_SPEED;
                break;
            default:
                break;
        }
        
        var newPos = getPos(currentX, currentY);
        PacStudent.transform.position = new Vector3(newPos.x, newPos.y, -1);

    }


}
