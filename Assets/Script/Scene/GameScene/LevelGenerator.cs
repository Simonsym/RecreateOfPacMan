using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    static readonly string TILE_PATH_BASE = "Tile/";
    public static readonly string[] TILE_MAPPING = {"empty", "outside_corner", "outside_wall", "inside_corner", "inside_wall", "normal_pellet", "power_pellet", "t"};
    public static readonly int [] ROTATE_INDEX = {360, 90, 180, 270};
    Dictionary<string, GameObject> prefabMapping;

    public static LevelGenerator instance;

    public GameObject p_bonus_score_cherry;
    public GameObject p_empty;
    public GameObject p_Ghost;
    public GameObject p_inside_corner;
    public GameObject p_inside_wall;
    public GameObject p_life_indicator;
    public GameObject p_NormalPellet;
    public GameObject p_outside_corner;
    public GameObject p_outside_wall;
    public GameObject p_PowerPellet;
    public GameObject p_t;

    public delegate void PutElement(string type, Vector2 position);

    Tilemap gameTileMap;
    Grid gameGrid;

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
    public int[,] extendedLevelMap = new int[30, 28];

    int[,] rotateMap = {
        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0}, 
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, 
        {0, 0, 0, 3, 3, 3, 0, 0, 3, 3, 3, 3, 0, 0}, 
        {0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 2, 0, 0}, 
        {0, 1, 1, 1, 1, 2, 0, 1, 1, 1, 1, 2, 0, 1}, 
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, 
        {0, 0, 0, 3, 3, 3, 0, 0, 3, 0, 0, 3, 3, 3}, 
        {0, 0, 1, 1, 1, 2, 0, 0, 0, 0, 1, 1, 1, 3}, 
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 2}, 
        {1, 1, 1, 1, 1, 3, 0, 0, 1, 1, 1, 3, 0, 2}, 
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 3, 2, 0, 1}, 
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, 
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 3, 0}, 
        {1, 1, 1, 3, 3, 2, 0, 1, 2, 0, 0, 0, 0, 0}, 
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
    };

    int[,] extendedRotateMap = new int[30, 28];

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        prefabMapping = new Dictionary<string, GameObject>
        {
            { "bonus_score_cherry", p_bonus_score_cherry },
            { "empty", p_empty },
            { "Ghost", p_Ghost },
            { "inside_corner", p_inside_corner },
            { "inside_wall", p_inside_wall },
            { "life_indicator", p_life_indicator },
            { "NormalPellet", p_NormalPellet },
            { "normal_pellet", p_NormalPellet },
            { "outside_corner", p_outside_corner },
            { "outside_wall", p_outside_wall },
            { "PowerPellet", p_PowerPellet },
            { "power_pellet", p_PowerPellet },
            { "t", p_t }
        };
    }

    // Update is called once per frame
    void Update() { }

    public void GeneratorMap(PutElement callback)
    {

        gameTileMap = GameObject.Find("GameTilemap").GetComponent<Tilemap>();
        gameGrid = GameObject.Find("GameGrid").GetComponent<Grid>();

        // ---- 14 ----
        // |
        // |
        // 15
        // |
        // |

        ExtendLevelMap();

        for(int r = 0; r < extendedLevelMap.GetLength(0); r++) {
            for(int c = 0; c < extendedLevelMap.GetLength(1); c++) {
                var newCoordinate = coordinateMapping(c, r);
                var tileValue = extendedLevelMap[r, c];

                var globalCoordinate = gameTileMap.CellToWorld(new Vector3Int(newCoordinate.x, newCoordinate.y, 0));

                if(TILE_MAPPING[tileValue].EndsWith("_pellet")) {
                    if(callback != null) {
                        callback(TILE_MAPPING[tileValue], new Vector2(globalCoordinate.x + 0.50f, globalCoordinate.y + 0.50f));
                    }
                }

                Quaternion quaternion;

                if(newCoordinate.x >= 14 && newCoordinate.y > -15)      { quaternion = Quaternion.Euler(0f, 180f, 0f); }
                else if(newCoordinate.x < 14 && newCoordinate.y > -15)  { quaternion = Quaternion.identity; }
                else if (newCoordinate.y < -15 && newCoordinate.x < 14) { quaternion = Quaternion.Euler(180f, 0f, 0f); } 
                else                                                 { quaternion = Quaternion.Euler(0f, 180f, 0f) * Quaternion.Euler(180f, 0f, 0f); }

                Instantiate(prefabMapping[TILE_MAPPING[tileValue]], new Vector3(globalCoordinate.x + 0.50f, globalCoordinate.y + 0.50f, 0), quaternion * Quaternion.Euler(0f, 0f, ROTATE_INDEX[extendedRotateMap[r, c]]));
            }
        }
    }

    public TileInfo queryTileInfoBoard(float x, float y) {
        return queryTileInfo(x, -y);
    }

    public TileInfo queryTileInfoBoard(int x, int y) {
        return queryTileInfo(x, -y);
    }

    public TileInfo queryTileInfo(int x, int y) {
        int type = extendedLevelMap[y, x];
        int rotate = extendedRotateMap[y, x];

        return new TileInfo(type, rotate);
    }

    public TileInfo queryTileInfo(float fx, float fy) {
        int x = (int)fx;
        int y = (int)fy;

        int type = 0;
        int rotate = 2;

        try {
            type = extendedLevelMap[y, x];
            rotate = extendedRotateMap[y, x];
        }
        catch(Exception _) { }

        return new TileInfo(type, rotate);
    }

    void ExtendLevelMap() {
        for(int r = 0; r < levelMap.GetLength(0); r++) {
            for(int c = 0; c < levelMap.GetLength(1); c++) {
                extendedLevelMap[r, c] = levelMap[r, c];
                extendedRotateMap[r, c] = rotateMap[r, c];
            }
            
        }
        // X O
        // O O

        for(int r = 0; r < levelMap.GetLength(0); r++) {
            for(int c = 0; c < levelMap.GetLength(1); c++) {
                var mirrorPos = extendedLevelMap.GetLength(1) - 1 - c;
                extendedLevelMap[r, mirrorPos] = levelMap[r, c];
                extendedRotateMap[r, mirrorPos] = rotateMap[r, c];
            }
        }

        // V X
        // O O

        for(int r = 0; r < extendedLevelMap.GetLength(0); r++) {
            for(int c = 0; c < extendedLevelMap.GetLength(1); c++) {
                var mirrorPos = extendedLevelMap.GetLength(0) - 1 - r;
                extendedLevelMap[mirrorPos, c] = extendedLevelMap[r, c];
                extendedRotateMap[mirrorPos, c] = extendedRotateMap[r, c];
            }
        }

        // V V
        // X X

        /*
        string content = "";

        for(int r = 0; r < extendedLevelMap.GetLength(0); r++) {
            for(int c = 0; c < extendedLevelMap.GetLength(1); c++) {
                content += extendedLevelMap[r, c];
                content += " ";
            }
            content += "\n";
        }
        */

    }

    Tile GetTileByIndex(int index) {
        return GetTileByName(TILE_MAPPING[index]);
    }

    Tile GetTileByName(string name)
    {
        return (Tile) Resources.Load(TILE_PATH_BASE + name, typeof(Tile));
    }
    Vector2Int coordinateMapping(int x, int y) {
        return new Vector2Int(x, -y);
    }

}

public class TileInfo {
    public int tileType;
    public string tileTypeStr;
    public int rotate;

    public TileInfo(int tileType, int rotate) {
        this.tileType = tileType;
        tileTypeStr = LevelGenerator.TILE_MAPPING[this.tileType];
        this.rotate = LevelGenerator.ROTATE_INDEX[rotate];
    }

    public override string ToString()
    {
        return $"TileInfo{{type={tileTypeStr} rotate={rotate}}}";
    }

}
