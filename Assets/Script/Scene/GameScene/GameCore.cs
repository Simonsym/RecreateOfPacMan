using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

using UnityEditor;
using UnityEditor.Animations;
using TMPro;

public class GameCore : MonoBehaviour
{

    static readonly string GAME_BACKGROUND = "Sound/game_background";
    
    Tilemap gameTileMap;
    Grid gameGrid;
    AudioClip backgroundSound;
    public GameObject PowerPelletPrefab;
    public GameObject GhostPrefab;
    public GameObject NormalPelletPrefab;

    public GameObject GhostR;
    public GameObject GhostP;
    public GameObject GhostB;
    public GameObject GhostY;
    public GameObject DieSparkPrefab;

    GameObject PacStudent;

    public float startTime { get; set; }
    public int score { get; set; }
    public int health_point { get; set; }

    LevelGenerator levelGeneratorScript;

    TextMeshProUGUI u_score;
    AudioSource backgroundPlayer;
    AudioSource audioSourceBrust;
    AudioClip audioClipEatPellet;
    AudioClip bgmScared;

    bool flagPowerPill = false;
    GameUI gameUI;

    Dictionary<String, GameObject> ghostMapping;

    GameObject[] hearts;

    Vector2Int playerInitPosition;

    PacStudentController pacStudentController;


    // Start is called before the first frame update
    void Start()
    {
        gameUI = GetComponentInParent<GameUI>();
        backgroundSound = Resources.Load<AudioClip>(GAME_BACKGROUND);

        startTime = Time.time;
        score = 0;
        bgmScared = Resources.Load<AudioClip>("Sound/game_background_scared");

        gameTileMap = GameObject.Find("GameTilemap").GetComponent<Tilemap>();
        gameGrid = GameObject.Find("GameGrid").GetComponent<Grid>();

        levelGeneratorScript = GetComponentInParent<LevelGenerator>();

        var audioSources = GetComponents<AudioSource>();

        backgroundPlayer = audioSources[0];
        audioSourceBrust = audioSources[1];

        PlayIntroSound();
        
        Invoke("LevelGenerator_instance_GeneratorMap", 0.01f);
        Invoke("putPacStudent", 0.02f);

        u_score = GameObject.Find("u_score").GetComponent<TextMeshProUGUI>();


        audioClipEatPellet = Resources.Load<AudioClip>("EffectSound/eat_pellet");
        Debug.Log(audioClipEatPellet);
        audioSourceBrust.clip = audioClipEatPellet;
        audioSourceBrust.loop = false;

        ghostMapping = new Dictionary<string, GameObject>() {
            {"GhostBlue", GhostB},
            {"GhostRed" , GhostR},
            {"GhostPink", GhostP},
            {"GhostYellow", GhostY}
        };

        putGhosts();
        setupGhosts();

        gameUI.showHideScare(false);

        hearts = new GameObject[3];

        hearts[0] = GameObject.Find("u_heart_0");
        hearts[1] = GameObject.Find("u_heart_1");
        hearts[2] = GameObject.Find("u_heart_2");

        health_point = 3;

    }

    // Update is called once per frame
    void Update() {
         // Debug.Log("audioSourceBrust.time: " + audioSourceBrust.time + "backgroundPlayer: " + backgroundPlayer.time);
    }

    public void onEatPellet(GameObject o) {
        switch(o.name) {
            case "normal_pellet": {
                audioSourceBrust.clip = audioClipEatPellet;
                audioSourceBrust.Play();
                
                addScore(10);
                Destroy(o);
                break ;
            }
            case "bonus_score_cherry": {
                audioSourceBrust.clip = audioClipEatPellet;
                audioSourceBrust.Play();
                
                addScore(100);
                Destroy(o);
                break ;
            }
            case "power_pellet": {
                onPowerPellet();
                Destroy(o);
                break ;
            }
        }   

    }

    public void onTouchGhost(GameObject o) {
        if(flagPowerPill) {

        }
        else {
            var PlayerPosition = PacStudent.transform.position;
            Instantiate(DieSparkPrefab, PlayerPosition, Quaternion.identity);
            health_point -= 1;
            Destroy(hearts[2]);
            pacStudentController.setupTransmit(playerInitPosition);


        }
    }

    public void addScore(int value) {
        // Debug.Log($"addScore {value} {Time.time}");
        score += value;
    }

    void setupFace(String ghostName, String faceName, bool flagAbsolute = false, String layerName = "Face Layer") {
        String fillFace;
        if(!flagAbsolute) {
            fillFace = ghostName + faceName;
        }
        else {
            fillFace = faceName;
        }
        ghostMapping[ghostName].GetComponentInChildren<Animator>().Play($"{layerName}.{fillFace}");

    }

    void setupGhosts() {
        setupGhosts("");
    }

    void setupGhosts(String face) {
        foreach(var kv in ghostMapping) {
            setupFace(kv.Key, face);
        }
    }

    void onPowerPellet() {
        flagPowerPill = true;

        backgroundPlayer.Stop();
        backgroundPlayer.clip = bgmScared;
        backgroundPlayer.Play(0);

        gameUI.setScare(10);
        Invoke("setGhostToRecovery", 7.0f);
        Invoke("stopPowerPelletMode", 10.0f);

        setupGhosts("Fear");


    }

    void setGhostToRecovery() {
        foreach(var kv in ghostMapping) {
            setupFace(kv.Key, "Flash", true, "Flash Layer");
        }
    }

    void stopPowerPelletMode() {
        flagPowerPill = false;

        backgroundPlayer.Stop();
        backgroundPlayer.clip = backgroundSound;
        backgroundPlayer.Play(0);

        setupGhosts("");

        foreach(var kv in ghostMapping) {
            setupFace(kv.Key, "NotFlash", true, "Flash Layer");
        }

    }

    void putGhosts() {
        Vector3 ghostPosition = new Vector3(11.5f, -12.5f, 0.0f);
        Vector3 direction     = new Vector3(1.0f,    0.0f, 0.0f);

        foreach(var kv in ghostMapping) {
            kv.Value.transform.position = ghostPosition;
            ghostPosition += direction;
        }

    }

    void PlayIntroSound() {
        backgroundPlayer.Play(0);
        Invoke("StopIntroSound", 5);
    }

    void StopIntroSound() {
        if (backgroundPlayer.clip == bgmScared) { return ; }

        backgroundPlayer.Stop();

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

        pacStudentController = PacStudent.GetComponent<PacStudentController>();

        pacStudentController.setupCurrentBoardPosition(availablePair.x, availablePair.y);

        playerInitPosition = availablePair;

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
