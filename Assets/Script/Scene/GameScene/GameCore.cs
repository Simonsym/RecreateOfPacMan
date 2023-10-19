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
    public GameObject PowerPelletPrefab;
    public GameObject GhostPrefab;
    public GameObject NormalPelletPrefab;

    public GameObject GhostR;
    public GameObject GhostG;
    public GameObject GhostB;
    public GameObject GhostY;

    GameObject PacStudent;

    public float startTime { get; set; }
    public int score { get; set; }

    LevelGenerator levelGeneratorScript;


    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        score = 0;

        gameTileMap = GameObject.Find("GameTilemap").GetComponent<Tilemap>();
        gameGrid = GameObject.Find("GameGrid").GetComponent<Grid>();

        levelGeneratorScript = GetComponentInParent<LevelGenerator>();

        PlayIntroSound();
        
        Invoke("LevelGenerator_instance_GeneratorMap", 0.01f);
        Invoke("putPacStudent", 0.02f);

        GhostR.transform.position = new Vector3(-0.75f, 0.45f, 0.0f);
        GhostG.transform.position = new Vector3(-0.45f, 0.45f, 0.0f);
        GhostB.transform.position = new Vector3(-0.15f, 0.45f, 0.0f);
        GhostY.transform.position = new Vector3( 0.15f, 0.45f, 0.0f);

    }

    // Update is called once per frame
    void Update() { }

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

    Vector2Int coordinateMapping(int x, int y) {
        return new Vector2Int(x, -y);
    }

    void putPacStudent() {
        List<int> availableType = new List<int>() {0, 5, 6};
        
        int availableX = -1;
        int availableY = -1;
        
        for(int r = 0; r < 15; r++) {
            for(int c = 0; c < 14; c++) {
                var tileInfo = levelGeneratorScript.queryTileInfo(c, r);
                if(availableType.Contains(tileInfo.tileType)) {
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

        PacStudent = GameObject.Find("go_pac_student");
        var availablePair = coordinateMapping(availableX, availableY);

        PacStudentController pacStudentController = PacStudent.GetComponent<PacStudentController>();

        pacStudentController.setupCurrentBoardPosition(availablePair.x, availablePair.y);

    }

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

    void LevelGenerator_instance_GeneratorMap() {
        levelGeneratorScript.GeneratorMap(delegateOfILevelGenerator);
    }

}
