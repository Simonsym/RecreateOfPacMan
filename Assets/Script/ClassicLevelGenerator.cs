using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ClassicLevelGenerator : MonoBehaviour, ILevelGenerator
{

    static readonly string TILE_PATH_BASE = "Tile/";

    readonly string[] TILE_MAPPING = {"empty", "outside_corner", "outside_wall", "inside_corner", "inside_wall", "normal_pellet", "power_pellet", "t"};

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

    int[,] extendedLevelMap = new int[30, 28];

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

    int [] rotateIndex = {360, 90, 180, 270};

    Dictionary<int, int> flipMapping = new Dictionary<int, int> {
        {0, 2},
        {1, 3},
        {2, 0},
        {3, 1}
    };

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


        string content = "";

        for(int r = 0; r < extendedLevelMap.GetLength(0); r++) {
            for(int c = 0; c < extendedLevelMap.GetLength(1); c++) {
                content += extendedLevelMap[r, c];
                content += " ";
            }
            content += "\n";
        }

        // Debug.Log(content);
    }

    Tile GetTileByIndex(int index) {
        return GetTileByName(TILE_MAPPING[index]);
    }

    Tile GetTileByName(string name)
    {
        return (Tile) Resources.Load(TILE_PATH_BASE + name, typeof(Tile));
    }
    Vector2Int coordinateMapping(int x, int y) {
        return new Vector2Int(x - 14, 14 - y);
    }

    public static ClassicLevelGenerator instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GeneratorMap(ILevelGenerator.PutElement callback)
    {
        if(Config.ENABLE_NEW_LEVEL_GENERATOR) {
            return ;
        }

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
                var tile = GetTileByIndex(tileValue);

                if(TILE_MAPPING[tileValue].EndsWith("_pellet")) {
                    if(callback != null) {
                        var globalCoordinate = gameTileMap.CellToWorld(new Vector3Int(newCoordinate.x, newCoordinate.y, 0));
                        callback(TILE_MAPPING[tileValue], new Vector2(globalCoordinate.x + 0.15f, globalCoordinate.y + 0.15f));
                        tile = GetTileByIndex(0);
                    }
                }

                

                gameTileMap.SetTile(new Vector3Int(newCoordinate.x, newCoordinate.y, 0), tile);


                Matrix4x4 matrix;

                if(newCoordinate.x >= 0 && newCoordinate.y > 0) {
                    matrix = Matrix4x4.Rotate(Quaternion.Euler(0f, 180f, 0f));
                }
                else if(newCoordinate.x < 0 && newCoordinate.y > 0) {
                    matrix = Matrix4x4.identity;
                }
                else if (newCoordinate.y < 0 && newCoordinate.x < 0) {
                    matrix = Matrix4x4.Rotate(Quaternion.Euler(180f, 0f, 0f));
                } 
                else /* newCoordinate.y < 0 && newCoordinate.x >= 0 */ {
                    matrix = Matrix4x4.Rotate(Quaternion.Euler(0f, 180f, 0f)) * Matrix4x4.Rotate(Quaternion.Euler(180f, 0f, 0f));
                }

                // 
                try {
                    gameTileMap.SetTransformMatrix(new Vector3Int(newCoordinate.x, newCoordinate.y, 0), matrix * Matrix4x4.Rotate(Quaternion.Euler(0f, 0f, rotateIndex[extendedRotateMap[r, c]])));
                }
                catch(Exception) {

                }

            }
        }
    }
}
