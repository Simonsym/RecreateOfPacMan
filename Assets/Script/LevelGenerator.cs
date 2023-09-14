using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
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

    InferableTile[,] inferableMap;

    int[,] extendedLevelMap = new int[30, 28];

    Vector2Int coordinateMapping(int x, int y) {
        return new Vector2Int(x - 14, 14 - y);
    }


    // Start is called before the first frame update
    void Start()
    {
        if(!Config.ENABLE_NEW_LEVEL_GENERATOR) {
            return ;
        }

        gameTileMap = GameObject.Find("GameTilemap").GetComponent<Tilemap>();
        gameGrid = GameObject.Find("GameGrid").GetComponent<Grid>();

        inferableMap = new InferableTile[15, 14];

        for(int r = 0; r < 15; r++) {
            for(int c = 0; c < 14; c++) {
                // var converted = coordinateMapping(c, r);
                inferableMap[r, c] = new InferableTile(c, r, levelMap[r, c]);
            }

        }


    }

    bool isLegal(int x, int y) {
        return x > InferableTile.EDGE_LEFT && 
            x < InferableTile.EDGE_RIGHT && 
            y > InferableTile.EDGE_BOTTOM && 
            y < InferableTile.EDGE_TOP;
    }

    void doInferable() {
        var currentInferableTile = inferableMap[0, 0];
        var stack = new Stack<Record>();
        var directions = currentInferableTile.getDirections();
        stack.Push(Record.n(currentInferableTile, directions[0]));
        stack.Push(Record.n(currentInferableTile, directions[1]));

        while (true)
        {
            var top = stack.Peek();
            int nextX = 0;
            int nextY = 0;
            switch(top.direction) {
                case InferableTile.UP: {
                    nextX = top.inferableTile.x;
                    nextY = top.inferableTile.y + 1;
                    break;
                }

                case InferableTile.DOWN: {
                    nextX = top.inferableTile.x;
                    nextY = top.inferableTile.y - 1;
                    break;
                }

                case InferableTile.LEFT: {
                    nextX = top.inferableTile.x - 1;
                    nextY = top.inferableTile.y;
                    break;
                }

                case InferableTile.RIGHT: {
                    nextX = top.inferableTile.x + 1;
                    nextY = top.inferableTile.y;
                    break;
                }
            }

            if(isLegal(nextX, nextY)) {

            }




        }



        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
